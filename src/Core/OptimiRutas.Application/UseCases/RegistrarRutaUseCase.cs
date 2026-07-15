namespace OptimiRutas.Application.UseCases;

using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.Mappers;
using OptimiRutas.Application.Ports;
using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.Services;
using OptimiRutas.Domain.ValueObjects;

public class RegistrarRutaUseCase
{
    private readonly IRutaRepository _repository;
    private readonly IOptimizadorRutaService _optimizador;
    private readonly Coordenada _puntoOrigen;

    public RegistrarRutaUseCase(IRutaRepository repository, IOptimizadorRutaService optimizador, Coordenada puntoOrigen)
    {
        _repository = repository;
        _optimizador = optimizador;
        _puntoOrigen = puntoOrigen;
    }

    public async Task<RutaDto> EjecutarAsync(RegistrarRutaCommand command, CancellationToken cancellationToken = default)
    {
        var nuevaRuta = new Ruta(RutaId.Nuevo(), command.Nombre, command.Repartidor);

        foreach (var paradaDto in command.Paradas)
        {
            var coordenada = new Coordenada(paradaDto.Latitud, paradaDto.Longitud);
            var direccion = new Direccion(paradaDto.Calle, coordenada);
            nuevaRuta.AgregarParada(direccion);
        }

        nuevaRuta.OptimizarRecorrido(_optimizador, _puntoOrigen);
        await _repository.GuardarAsync(nuevaRuta, cancellationToken);

        return RutaMapper.MapearARutaDto(nuevaRuta);
    }
}
