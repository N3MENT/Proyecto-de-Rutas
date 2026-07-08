namespace OptimiRutas.Application.UseCases;

using OptimiRutas.Application.DTOs;
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

        // 1. Buscamos la entidad en el repositorio
        var ruta = await _repository.ObtenerPorIdAsync(rutaId, cancellationToken);
        
        if (ruta is null)
            throw new KeyNotFoundException($"No se encontró ninguna ruta con el ID {command.RutaId}");

        // 2. Ejecutamos la lógica de negocio (el método lanza excepción si ya estaba completada)
        ruta.CompletarParada(paradaId);

        // 3. Guardamos la actualización
        await _repository.ActualizarAsync(ruta, cancellationToken);

        // 4. Retornamos el DTO actualizado con los nuevos porcentajes de progreso
        return RegistrarRutaUseCase.MapearARutaDto(ruta);
    }
}