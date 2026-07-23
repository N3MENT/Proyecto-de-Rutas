// OptimiRutas - App principal con mapa interactivo

const API_BASE = '';
const TOKEN_KEY = 'optimirutas_user';

// ============== ESTADO GLOBAL ==============
let map;
let userMarker;
let destMarker;
let origin = null;
let destination = null;
let selectedRoutePolyline = null;
let alternativePolylines = [];
let currentRouteOptions = [];
let isNavigating = false;
let watchId = null;
let vehiculoSeleccionado = 'Moto';
let userZoomed = false;
let searchCountryCode = '';

// ============== INICIALIZACIÓN ==============
document.addEventListener('DOMContentLoaded', () => {
    const user = getSession();
    if (!user) {
        window.location.href = 'index.html';
        return;
    }

    initUI(user);
    initMap();
    initEventListeners();
    initVehicleSelector();
    getUserLocation();
});

function getSession() {
    const raw = sessionStorage.getItem(TOKEN_KEY);
    return raw ? JSON.parse(raw) : null;
}

// ============== UI ==============
function initUI(user) {
    document.getElementById('userDisplay').textContent = user.nombre;
    document.getElementById('menuUserName').textContent = user.nombre;
    document.getElementById('menuUserEmail').textContent = user.email;
    document.getElementById('menuUserIcon').textContent = user.nombre.charAt(0).toUpperCase();
}

function initEventListeners() {
    document.getElementById('destinationInput').addEventListener('input', onSearchInput);
    document.getElementById('clearSearch').addEventListener('click', clearDestination);
    document.getElementById('destinationInput').addEventListener('keydown', (e) => {
        if (e.key === 'Enter') onSearchSubmit();
    });

    document.getElementById('locateBtn').addEventListener('click', getUserLocation);

    document.getElementById('menuBtn').addEventListener('click', toggleMenu);
    document.getElementById('menuOverlay').addEventListener('click', toggleMenu);

    document.getElementById('logoutBtn').addEventListener('click', () => {
        sessionStorage.removeItem(TOKEN_KEY);
        window.location.href = 'index.html';
    });

    document.getElementById('closeRoutes').addEventListener('click', closeRoutePanel);
    document.getElementById('cancelRoute').addEventListener('click', cancelRoute);
    document.getElementById('startNavigation').addEventListener('click', startNavigation);
}

function initVehicleSelector() {
    const options = document.querySelectorAll('.vehicle-option');
    options.forEach(opt => {
        opt.addEventListener('click', () => {
            options.forEach(o => o.classList.remove('selected'));
            opt.classList.add('selected');
            vehiculoSeleccionado = opt.dataset.vehiculo;
            if (origin && destination) {
                requestRoutes();
            }
        });
    });
}

function toggleMenu() {
    document.getElementById('sideMenu').classList.toggle('hidden');
    document.getElementById('menuOverlay').classList.toggle('hidden');
    // Recalcular tamaño del mapa tras mostrar/ocultar el menú
    setTimeout(() => { try { if (map) map.invalidateSize(); } catch (e) {} }, 350);
}

// ============== MAPA ==============
function initMap() {
    map = L.map('map', {
        zoomControl: false,
        attributionControl: false
    }).setView([10.2506, -66.8837], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19
    }).addTo(map);

    L.control.zoom({ position: 'bottomright' }).addTo(map);

    map.on('click', (e) => {
        setDestination(e.latlng);
    });

    map.on('zoomend', () => {
        userZoomed = true;
    });

    // Algunos navegadores calculan mal el tamaño del contenedor si está dentro
    // de un layout flexible al inicializarse. Forzamos una invalidación
    // del tamaño después de un pequeño retardo y al redimensionar.
    setTimeout(() => { try { map.invalidateSize(); } catch (e) {} }, 300);
    window.addEventListener('resize', () => { try { if (map) map.invalidateSize(); } catch (e) {} });
}

async function detectCountry(lat, lng) {
    try {
        const res = await fetch(
            `https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}&addressdetails=1`,
            { headers: { 'Accept-Language': 'es' } }
        );
        const data = await res.json();
        const code = data?.address?.country_code;
        if (code) {
            searchCountryCode = code;
        }
    } catch {}
}

function getUserLocation() {
    document.getElementById('locationStatus').innerHTML = `
        <span class="pulse-dot"></span>
        <span>Obteniendo ubicación...</span>
    `;

    if (!navigator.geolocation) {
        document.getElementById('locationStatus').innerHTML = `
            <span>⚠️</span>
            <span>Geolocalización no disponible</span>
        `;
        setOrigin([10.2506, -66.8837]);
        return;
    }

    alert('OptimiRutas necesita acceder a tu ubicación para calcular rutas. Por favor, concede el permiso cuando el navegador lo solicite.');

    navigator.geolocation.getCurrentPosition(
        (pos) => {
            const coords = [pos.coords.latitude, pos.coords.longitude];
            document.getElementById('locationStatus').innerHTML = `
                <span class="pulse-dot" style="background: #1e8e3e"></span>
                <span>Ubicación obtenida</span>
            `;
            setOrigin(coords);
            detectCountry(coords[0], coords[1]);
        },
        (err) => {
            console.warn('Geolocation error:', err.message);
            document.getElementById('locationStatus').innerHTML = `
                <span>⚠️</span>
                <span>Usando ubicación aproximada</span>
            `;
            setOrigin([10.2506, -66.8837]);
        },
        { enableHighAccuracy: true, timeout: 10000 }
    );
}

function setOrigin(coords) {
    origin = { lat: coords[0], lng: coords[1] };

    if (userMarker) map.removeLayer(userMarker);
    const originIcon = L.divIcon({
        className: 'custom-marker',
        html: `<div style="width:20px;height:20px;border-radius:50%;background:#1a73e8;border:3px solid white;box-shadow:0 2px 6px rgba(0,0,0,0.3);display:flex;align-items:center;justify-content:center;"><div style="width:8px;height:8px;border-radius:50%;background:white;"></div></div>`,
        iconSize: [20, 20],
        iconAnchor: [10, 10]
    });
    userMarker = L.marker(coords, { icon: originIcon, draggable: true }).addTo(map);
    userMarker.bindPopup('<b>Tu ubicación</b>');
    userMarker.on('dragend', function(e) {
        const pos = e.target.getLatLng();
        origin = { lat: pos.lat, lng: pos.lng };
        if (destination) requestRoutes();
    });
    map.setView(coords, 14);

    if (destination) {
        requestRoutes();
    }
}

function setDestination(latlng) {
    destination = { lat: latlng.lat, lng: latlng.lng };

    if (destMarker) map.removeLayer(destMarker);
    const destIcon = L.divIcon({
        className: 'custom-marker',
        html: `<div style="width:26px;height:26px;border-radius:50%;background:#d93025;border:3px solid white;box-shadow:0 2px 6px rgba(0,0,0,0.3);display:flex;align-items:center;justify-content:center;"><svg width="12" height="12" viewBox="0 0 24 24" fill="white"><circle cx="12" cy="12" r="6"/></svg></div>`,
        iconSize: [26, 26],
        iconAnchor: [13, 13]
    });
    destMarker = L.marker([latlng.lat, latlng.lng], { icon: destIcon, draggable: true }).addTo(map);
    destMarker.bindPopup('<b>Destino</b>');
    destMarker.on('dragend', function(e) {
        const pos = e.target.getLatLng();
        destination = { lat: pos.lat, lng: pos.lng };
        if (origin) requestRoutes();
    });

    document.getElementById('clearSearch').classList.remove('hidden');
    document.getElementById('mapHint').classList.add('hidden');
    document.getElementById('suggestions').classList.add('hidden');

    if (origin) {
        requestRoutes();
    }
}

function clearDestination() {
    if (destMarker) { map.removeLayer(destMarker); destMarker = null; }
    destination = null;
    currentRouteOptions = [];
    document.getElementById('destinationInput').value = '';
    document.getElementById('clearSearch').classList.add('hidden');
    closeRoutePanel();
    cancelRoute();
    clearRoutePolylines();
}

// ============== BÚSQUEDA (Nominatim) ==============
let searchTimer = null;

function onSearchInput() {
    const q = document.getElementById('destinationInput').value.trim();
    if (q.length < 3) {
        document.getElementById('suggestions').classList.add('hidden');
        return;
    }

    clearTimeout(searchTimer);
    searchTimer = setTimeout(() => geocodeSearch(q), 400);
}

async function geocodeSearch(q) {
    let url = `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(q)}&limit=5&addressdetails=1`;
    if (searchCountryCode) {
        url += `&countrycodes=${searchCountryCode}`;
    }
    try {
        const res = await fetch(url, { headers: { 'Accept-Language': 'es' } });
        const data = await res.json();
        showSuggestions(data);
    } catch {}
}

function showSuggestions(results) {
    const list = document.getElementById('suggestions');
    list.innerHTML = '';

    if (results.length === 0) {
        list.classList.add('hidden');
        return;
    }

    results.forEach(r => {
        const item = document.createElement('div');
        item.className = 'suggestion-item';
        item.innerHTML = `
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#999" stroke-width="2">
                <circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/>
            </svg>
            <div>
                <div class="suggestion-name">${r.display_name.split(',')[0]}</div>
                <div class="suggestion-addr">${r.display_name}</div>
            </div>
        `;
        item.addEventListener('click', () => {
            setDestination({ lat: parseFloat(r.lat), lng: parseFloat(r.lon) });
            document.getElementById('destinationInput').value = r.display_name.split(',')[0];
            list.classList.add('hidden');
        });
        list.appendChild(item);
    });

    list.classList.remove('hidden');
}

function onSearchSubmit() {
    const q = document.getElementById('destinationInput').value.trim();
    if (q.length > 0) {
        document.getElementById('suggestions').classList.add('hidden');
        document.getElementById('mapHint').classList.remove('hidden');
    }
}

// ============== RUTAS ==============
async function requestRoutes() {
    if (!origin || !destination) return;

    document.getElementById('loadingOverlay').classList.remove('hidden');
    closeRoutePanel();
    clearRoutePolylines();

    try {
        const res = await fetch(`${API_BASE}/api/rutas/sugerir`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                origenLat: origin.lat,
                origenLng: origin.lng,
                destinoLat: destination.lat,
                destinoLng: destination.lng,
                nombreDestino: document.getElementById('destinationInput').value || 'Destino',
                vehiculo: vehiculoSeleccionado
            })
        });

        if (!res.ok) throw new Error('Error al obtener rutas');

        currentRouteOptions = await res.json();
        showRouteOptions(currentRouteOptions);
        showAllRoutesOnMap(currentRouteOptions);
    } catch (err) {
        console.error(err);
        alert('No se pudieron calcular las rutas. Intenta de nuevo.');
    } finally {
        document.getElementById('loadingOverlay').classList.add('hidden');
    }
}

function showRouteOptions(options) {
    const list = document.getElementById('routeList');
    list.innerHTML = '';
    const panel = document.getElementById('routeOptions');
    panel.classList.remove('hidden');

    options.forEach((ruta, idx) => {
        const tiempo = ruta.minutosEstimados < 60 ? `${ruta.minutosEstimados} min` : `${Math.floor(ruta.minutosEstimados / 60)}h ${ruta.minutosEstimados % 60}m`;
        const tags = ['Recomendada', 'Alternativa', 'Económica'];
        const card = document.createElement('div');
        card.className = `route-card ${idx === 0 ? 'selected' : ''}`;
        card.innerHTML = `
            <div class="route-card-name">${ruta.icono || ''} ${ruta.nombre}</div>
            <div class="route-card-details">
                <span>🕐 ${tiempo}</span>
                <span>📏 ${ruta.distanciaKm} km</span>
                <span>📍 ${ruta.totalParadas} paradas</span>
            </div>
            <span class="route-card-tag">${tags[idx]}</span>
        `;
        card.addEventListener('click', () => selectRoute(idx));
        list.appendChild(card);
    });

    selectRoute(0);
}

function selectRoute(idx) {
    const cards = document.querySelectorAll('.route-card');
    cards.forEach((c, i) => c.classList.toggle('selected', i === idx));

    highlightRoute(idx);

    const ruta = currentRouteOptions[idx];
    if (ruta) {
        const summary = document.getElementById('routeSummary');
        summary.classList.remove('hidden');
        const tiempo = ruta.minutosEstimados < 60 ? `${ruta.minutosEstimados} min` : `${Math.floor(ruta.minutosEstimados / 60)}h ${ruta.minutosEstimados % 60}m`;
        document.getElementById('summaryRouteName').textContent = `${ruta.icono || ''} ${ruta.nombre}`;
        document.getElementById('summaryTime').textContent = `${tiempo}`;
        document.getElementById('summaryDistance').textContent = `${ruta.distanciaKm} km`;
    }
}

function closeRoutePanel() {
    document.getElementById('routeOptions').classList.add('hidden');
    document.getElementById('routeSummary').classList.add('hidden');
}

function cancelRoute() {
    isNavigating = false;
    closeRoutePanel();

    if (watchId !== null) {
        navigator.geolocation.clearWatch(watchId);
        watchId = null;
    }

    document.getElementById('startNavigation').textContent = 'Iniciar';

    // Quitar todas las polilíneas del mapa y marcar que el usuario hizo zoom manual
    clearRoutePolylines();
    userZoomed = true;
}

function startNavigation() {
    if (!isNavigating) {
        isNavigating = true;
        document.getElementById('startNavigation').textContent = '✓ Navegando';

        // Al iniciar navegación, centrar en el usuario sin cambiar zoom
        userZoomed = true;
        if (userMarker) {
            map.panTo(userMarker.getLatLng(), { animate: true, duration: 0.5 });
        }

        watchId = navigator.geolocation.watchPosition(
            (pos) => {
                const coords = [pos.coords.latitude, pos.coords.longitude];
                if (userMarker) {
                    userMarker.setLatLng(coords);
                }
                // Solo mover la vista, nunca cambiar zoom
                map.panTo(coords, { animate: true, duration: 1 });
            },
            () => {},
            { enableHighAccuracy: true, timeout: 5000 }
        );
    } else {
        cancelRoute();
    }
}

// ============== MAPA - POLILÍNEAS ==============
function clearRoutePolylines() {
    if (selectedRoutePolyline) {
        map.removeLayer(selectedRoutePolyline);
        selectedRoutePolyline = null;
    }
    alternativePolylines.forEach(p => map.removeLayer(p));
    alternativePolylines = [];
}

function showAllRoutesOnMap(options) {
    clearRoutePolylines();

    options.forEach((ruta, idx) => {
        const puntos = ruta.paradas ? ruta.paradas.map(p => [p.lat, p.lng]) : [];
        const fullPath = [[origin.lat, origin.lng], ...puntos];

        const color = idx === 0 ? '#1a73e8' : idx === 1 ? '#e8a317' : '#34a853';
        const weight = idx === 0 ? 5 : 3;
        const opacity = idx === 0 ? 0.8 : 0.5;
        const dashArray = idx === 0 ? '' : '8, 8';

        const polyline = L.polyline(fullPath, { color, weight, opacity, dashArray }).addTo(map);

        if (idx === 0) selectedRoutePolyline = polyline;
        else alternativePolylines.push(polyline);
    });

    // Solo ajustar vista si el usuario no ha hecho zoom manual
    if (!userZoomed) {
        const allBounds = L.latLngBounds([origin.lat, origin.lng], [destination.lat, destination.lng]);
        map.fitBounds(allBounds, { padding: [60, 60] });
    }
}

function highlightRoute(idx) {
    document.querySelectorAll('.route-card').forEach((c, i) => c.classList.toggle('selected', i === idx));
}

// ============== ESTILOS ADICIONALES ==============
(() => {
    const style = document.createElement('style');
    style.textContent = `
        .custom-marker { background: transparent; border: none; }
        .custom-marker div { cursor: grab; }
        .custom-marker div:active { cursor: grabbing; }
        .suggestions-list {
            background: white;
            border-top: 1px solid var(--border);
            max-height: 200px;
            overflow-y: auto;
        }
        .suggestion-item {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 10px 14px;
            cursor: pointer;
            transition: background 0.15s;
            font-size: 13px;
        }
        .suggestion-item:hover { background: var(--primary-light); }
        .suggestion-name { font-weight: 500; color: var(--text); }
        .suggestion-addr { font-size: 11px; color: var(--text-secondary); margin-top: 2px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 250px; }

        @media (max-width: 767px) {
            .suggestion-addr { max-width: 180px; }
        }
    `;
    document.head.appendChild(style);
})();
