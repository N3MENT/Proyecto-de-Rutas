// OptimiRutas - Auth (login/registro)

const API_BASE = '';
const TOKEN_KEY = 'optimirutas_user';

document.addEventListener('DOMContentLoaded', () => {
    const tabs = document.querySelectorAll('.tab-btn');
    const forms = document.querySelectorAll('.auth-form');
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');

    // Redirigir si ya inició sesión
    if (getSession()) {
        window.location.href = 'app.html';
        return;
    }

    // Tabs
    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            tabs.forEach(t => t.classList.remove('active'));
            forms.forEach(f => f.classList.remove('active'));
            tab.classList.add('active');
            document.getElementById(tab.dataset.tab + 'Form').classList.add('active');
            clearErrors();
        });
    });

    // Login
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        clearErrors();

        const email = document.getElementById('loginEmail').value.trim();
        const password = document.getElementById('loginPassword').value;

        const btn = loginForm.querySelector('button[type="submit"]');
        btn.disabled = true;
        btn.textContent = 'Entrando...';

        try {
            const res = await fetch(`${API_BASE}/api/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, contrasena: password })
            });

            if (!res.ok) {
                const err = await res.json();
                showError('loginError', err.error || 'Credenciales inválidas');
                return;
            }

            const user = await res.json();
            saveSession(user);
            window.location.href = 'app.html';
        } catch {
            showError('loginError', 'Error de conexión. Verifica tu red.');
        } finally {
            btn.disabled = false;
            btn.textContent = 'Entrar';
        }
    });

    // Register
    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        clearErrors();

        const nombre = document.getElementById('regName').value.trim();
        const email = document.getElementById('regEmail').value.trim();
        const password = document.getElementById('regPassword').value;

        if (!nombre) { showError('registerError', 'El nombre es obligatorio.'); return; }
        if (nombre.length < 3) { showError('registerError', 'El nombre debe tener al menos 3 caracteres.'); return; }
        if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(nombre)) { showError('registerError', 'El nombre solo debe contener letras y espacios.'); return; }
        if (nombre.split(/\s+/).length < 2) { showError('registerError', 'Ingresa tu nombre y apellido (al menos 2 palabras).'); return; }
        if (!email) { showError('registerError', 'El correo electrónico es obligatorio.'); return; }
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) { showError('registerError', 'Ingresa un correo electrónico válido.'); return; }
        if (password.length < 8) { showError('registerError', 'La contraseña debe tener al menos 8 caracteres.'); return; }
        if (!/[A-Z]/.test(password)) { showError('registerError', 'La contraseña debe contener al menos una letra mayúscula.'); return; }
        if (!/[a-z]/.test(password)) { showError('registerError', 'La contraseña debe contener al menos una letra minúscula.'); return; }
        if (!/[0-9]/.test(password)) { showError('registerError', 'La contraseña debe contener al menos un número.'); return; }

        const btn = registerForm.querySelector('button[type="submit"]');
        btn.disabled = true;
        btn.textContent = 'Creando cuenta...';

        try {
            const res = await fetch(`${API_BASE}/api/auth/registrar`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ nombre, email, contrasena: password })
            });

            if (!res.ok) {
                const err = await res.json();
                showError('registerError', err.error || 'Error al registrarse');
                return;
            }

            const user = await res.json();
            document.getElementById('registerSuccess').classList.remove('hidden');
            document.getElementById('registerSuccess').textContent = '¡Cuenta creada! Ahora inicia sesión.';

            setTimeout(() => {
                document.querySelectorAll('.tab-btn').forEach(t => t.classList.remove('active'));
                document.querySelectorAll('.auth-form').forEach(f => f.classList.remove('active'));
                document.querySelector('[data-tab="login"]').classList.add('active');
                document.getElementById('loginForm').classList.add('active');
                document.getElementById('regName').value = '';
                document.getElementById('regEmail').value = '';
                document.getElementById('regPassword').value = '';
                clearErrors();
            }, 1500);
        } catch {
            showError('registerError', 'Error de conexión. Verifica tu red.');
        } finally {
            btn.disabled = false;
            btn.textContent = 'Crear Cuenta';
        }
    });
});

function showError(id, msg) {
    const el = document.getElementById(id);
    el.textContent = msg;
    el.classList.remove('hidden');
}

function clearErrors() {
    document.querySelectorAll('.error-msg').forEach(e => {
        e.classList.add('hidden');
        e.textContent = '';
    });
    document.querySelectorAll('.success-msg').forEach(e => {
        e.classList.add('hidden');
        e.textContent = '';
    });
}

function saveSession(user) {
    sessionStorage.setItem(TOKEN_KEY, JSON.stringify(user));
}

function getSession() {
    const raw = sessionStorage.getItem(TOKEN_KEY);
    return raw ? JSON.parse(raw) : null;
}
