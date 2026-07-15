namespace OptimiRutas.Application.UseCases;

using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.Mappers;
using OptimiRutas.Application.Ports;

public class ObtenerTodasLasRutasUseCase
{
    private readonly IRutaRepository _repository;

    public ObtenerTodasLasRutasUseCase(IRutaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RutaDto>> EjecutarAsync(CancellationToken cancellationToken = default)
    {
        var rutas = await _repository.ObtenerTodasAsync(cancellationToken);
        return rutas.Select(RutaMapper.MapearARutaDto);
    }
}
