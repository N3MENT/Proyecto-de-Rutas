namespace OptimiRutas.Application.Ports;

using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Usuario?> ObtenerPorIdAsync(UsuarioId id, CancellationToken cancellationToken = default);
    Task GuardarAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
