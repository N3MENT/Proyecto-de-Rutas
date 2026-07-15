namespace OptimiRutas.Application.UseCases;

using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.Mappers;
using OptimiRutas.Application.Ports;
using OptimiRutas.Domain.ValueObjects;

public class ObtenerRutaPorIdUseCase
{
    private readonly IRutaRepository _repository;

    public ObtenerRutaPorIdUseCase(IRutaRepository repository)
    {
        _repository = repository;
    }

    public async Task<RutaDto?> EjecutarAsync(Guid rutaId, CancellationToken cancellationToken = default)
    {
        var ruta = await _repository.ObtenerPorIdAsync(new RutaId(rutaId), cancellationToken);
        return ruta is null ? null : RutaMapper.MapearARutaDto(ruta);
    }
}
