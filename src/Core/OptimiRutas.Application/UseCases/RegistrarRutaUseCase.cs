namespace OptimiRutas.Application.UseCases;

using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.Ports;
using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.Services;
using OptimiRutas.Domain.ValueObjects;

public class RegistrarRutaUseCase
{
    private readonly IRutaRepository _repository;
    private readonly IOptimizadorRutaService _optimizador;

    // Inyectamos las interfaces (Puertos), nunca implementaciones concretas
    public RegistrarRutaUseCase(IRutaRepository repository, IOptimizadorRutaService optimizador)
    {
        _repository = repository;
        _optimizador = optimizador;
    }

    public async Task<RutaDto> EjecutarAsync(RegistrarRutaCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Instanciamos nuestra entidad raíz del dominio
        var nuevaRuta = new Ruta(RutaId.Nuevo(), command.Nombre, command.Repartidor);

        // 2. Agregamos cada parada usando los Value Objects
        foreach (var paradaDto in command.Paradas)
        {
            var coordenada = new Coordenada(paradaDto.Latitud, paradaDto.Longitud);
            var direccion = new Direccion(paradaDto.Calle, coordenada);
            nuevaRuta.AgregarParada(direccion);
        }

        // 3. Orquestamos la optimización con la IA/Algoritmo
        // (Definimos un punto base de salida para el comercio local: ej. Coordenadas 300, 225)
        var puntoCentralNegocio = new Coordenada(225, 300);
        nuevaRuta.OptimizarRecorrido(_optimizador, puntoCentralNegocio);

        // 4. Persistimos los cambios a través del Puerto
        await _repository.GuardarAsync(nuevaRuta, cancellationToken);

        // 5. Mapeamos la entidad de dominio a un DTO de salida
        return MapearARutaDto(nuevaRuta);
    }

    // Método auxiliar de mapeo (En proyectos grandes se puede usar una librería como AutoMapper o Mapster)
    public static RutaDto MapearARutaDto(Ruta ruta)
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

        var stats = ruta.ObtenerEstadisticas();
        var statsDto = new EstadisticasDto(stats.Total, stats.Completadas, stats.MinutosEstimados, stats.AhorroEstimado);

        return new RutaDto(
            ruta.Id.Value, 
            ruta.Nombre, 
            ruta.Repartidor, 
            ruta.CalcularProgreso(), 
            statsDto, 
            paradasDto
        );
    }
}