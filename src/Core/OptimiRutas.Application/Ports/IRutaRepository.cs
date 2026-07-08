namespace OptimiRutas.Application.Ports;

using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;

public interface IRutaRepository
{
    Task<Ruta?> ObtenerPorIdAsync(RutaId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Ruta>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task GuardarAsync(Ruta ruta, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Ruta ruta, CancellationToken cancellationToken = default);
}