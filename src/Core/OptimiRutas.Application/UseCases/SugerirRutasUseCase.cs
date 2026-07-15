namespace OptimiRutas.Application.UseCases;

using OptimiRutas.Application.DTOs;
using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.Services;
using OptimiRutas.Domain.ValueObjects;

public class SugerirRutasUseCase
{
    private readonly IOptimizadorRutaService _optimizador;

    // Velocidades medias en km/h para cada tipo de vehículo en entorno urbano/interurbano
    private static readonly Dictionary<string, (double VeloRapida, double VeloAlternativa, string Icono)> Velocidades = new()
    {
        ["Moto"] = (65, 55, "🏍️"),
        ["Carro"] = (55, 45, "🚗"),
        ["Camioneta"] = (50, 40, "🚐"),
        ["Bicicleta"] = (18, 15, "🚲"),
        ["Caminando"] = (5, 4, "🚶"),
    };

    // Factor de tráfico: reduce velocidad un % en hora pico
    private const double FactorTraficoBajo = 0.90;
    private const double FactorTraficoAlto = 0.60;

    public SugerirRutasUseCase(IOptimizadorRutaService optimizador)
    {
        _optimizador = optimizador;
    }

    public Task<List<OpcionRutaDto>> EjecutarAsync(SugerirRutaCommand command, CancellationToken cancellationToken = default)
    {
        var origen = new Coordenada(command.OrigenLat, command.OrigenLng);
        var destino = new Coordenada(command.DestinoLat, command.DestinoLng);
        var nombreDestino = command.NombreDestino ?? "Destino seleccionado";

        // Obtener velocidades según vehículo
        if (!Velocidades.TryGetValue(command.Vehiculo, out var vel))
        {
            vel = Velocidades["Carro"];
        }

        // Factor de tráfico según hora del día
        var hora = DateTime.Now.Hour;
        var factorTrafico = EsHoraPico(hora) ? FactorTraficoAlto : FactorTraficoBajo;
        if (command.EvitarTrafico)
            factorTrafico = 1.0;

        var velRapida = vel.VeloRapida * factorTrafico;
        var velAlternativa = vel.VeloAlternativa * factorTrafico;

        var opciones = new List<OpcionRutaDto>();

        // Opción 1: Ruta más rápida (directa, mejor velocidad)
        var rutaDirecta = CalcularRuta(origen, destino, nombreDestino, "Más rápida", 0, velRapida, vel.Icono);
        opciones.Add(rutaDirecta);

        // Opción 2: Ruta alternativa con punto intermedio (evita tráfico)
        var puntoIntermedio = DesplazarCoordenada(origen, destino, 0.15, -0.12);
        var rutaAlternativa = CalcularRutaAlternativa(origen, puntoIntermedio, destino, nombreDestino, "Alternativa", 1, velAlternativa, vel.Icono);
        opciones.Add(rutaAlternativa);

        // Opción 3: Ruta panorámica/económica (más larga pero escénica)
        var puntoPanoramico = DesplazarCoordenada(origen, destino, -0.10, 0.20);
        var rutaPanoramica = CalcularRutaAlternativa(origen, puntoPanoramico, destino, nombreDestino, "Económica", 2, velAlternativa * 0.85, vel.Icono);
        opciones.Add(rutaPanoramica);

        return Task.FromResult(opciones);
    }

    private bool EsHoraPico(int hora)
    {
        // Horas pico: 6-9am y 4-7pm
        return (hora >= 6 && hora <= 9) || (hora >= 16 && hora <= 19);
    }

    private OpcionRutaDto CalcularRuta(Coordenada origen, Coordenada destino, string nombreDestino, string nombreRuta, int variacion, double velocidadKmh, string icono)
    {
        var ruta = new Ruta(RutaId.Nuevo(), nombreRuta, "Sistema");
        ruta.AgregarParada(new Direccion(nombreDestino, destino));
        ruta.OptimizarRecorrido(_optimizador, origen);

        var distancia = CalcularDistanciaKm(origen, destino) * (1 + variacion * 0.02);
        var minutos = (int)Math.Round((distancia / velocidadKmh) * 60);

        return MapearARutaDto(ruta, minutos, distancia, icono);
    }

    private OpcionRutaDto CalcularRutaAlternativa(Coordenada origen, Coordenada intermedio, Coordenada destino, string nombreDestino, string nombreRuta, int variacion, double velocidadKmh, string icono)
    {
        var ruta = new Ruta(RutaId.Nuevo(), nombreRuta, "Sistema");
        ruta.AgregarParada(new Direccion("Punto intermedio", intermedio));
        ruta.AgregarParada(new Direccion(nombreDestino, destino));
        ruta.OptimizarRecorrido(_optimizador, origen);

        var distancia = (CalcularDistanciaKm(origen, intermedio) + CalcularDistanciaKm(intermedio, destino)) * (1 + variacion * 0.02);
        var minutos = (int)Math.Round((distancia / velocidadKmh) * 60);

        return MapearARutaDto(ruta, minutos, distancia, icono);
    }

    private static Coordenada DesplazarCoordenada(Coordenada a, Coordenada b, double factorLat, double factorLng)
    {
        var difLat = b.Latitud - a.Latitud;
        var difLng = b.Longitud - a.Longitud;
        return new Coordenada(
            a.Latitud + difLat * 0.5 + factorLat,
            a.Longitud + difLng * 0.5 + factorLng
        );
    }

    private static double CalcularDistanciaKm(Coordenada a, Coordenada b)
    {
        var R = 6371.0;
        var dLat = GradosARadianes(b.Latitud - a.Latitud);
        var dLng = GradosARadianes(b.Longitud - a.Longitud);
        var x = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(GradosARadianes(a.Latitud)) * Math.Cos(GradosARadianes(b.Latitud)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(x), Math.Sqrt(1 - x));
        return R * c;
    }

    private static double GradosARadianes(double grados) => grados * Math.PI / 180;

    private static OpcionRutaDto MapearARutaDto(Ruta ruta, int minutos, double distancia, string icono)
    {
        var paradasDto = ruta.Paradas
            .OrderBy(p => p.Orden)
            .Select(p => new ParadaDto(
                p.Id.Value,
                p.Direccion.Calle,
                p.Direccion.Coordenada.Latitud,
                p.Direccion.Coordenada.Longitud,
                p.Completada,
                p.Orden))
            .ToList();

        return new OpcionRutaDto(
            ruta.Id.Value,
            ruta.Nombre,
            ruta.Paradas.Count,
            minutos < 1 ? 1 : minutos,
            Math.Round(distancia, 2),
            icono,
            paradasDto
        );
    }
}
