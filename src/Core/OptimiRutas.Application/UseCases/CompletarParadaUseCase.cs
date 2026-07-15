namespace OptimiRutas.Application.UseCases;

using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.Mappers;
using OptimiRutas.Application.Ports;
using OptimiRutas.Domain.ValueObjects;

public class CompletarParadaUseCase
{
    private readonly IRutaRepository _repository;

    public CompletarParadaUseCase(IRutaRepository repository)
    {
        _repository = repository;
    }

    public async Task<RutaDto> EjecutarAsync(CompletarParadaCommand command, CancellationToken cancellationToken = default)
    {
        var rutaId = new RutaId(command.RutaId);
        var paradaId = new ParadaId(command.ParadaId);

        var ruta = await _repository.ObtenerPorIdAsync(rutaId, cancellationToken);

        if (ruta is null)
            throw new KeyNotFoundException($"No se encontró ninguna ruta con el ID {command.RutaId}");

        ruta.CompletarParada(paradaId);
        await _repository.ActualizarAsync(ruta, cancellationToken);

        return RutaMapper.MapearARutaDto(ruta);
    }
}
