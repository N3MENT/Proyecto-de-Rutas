namespace OptimiRutas.Infrastructure.Persistence;

using System.Collections.Concurrent;
using OptimiRutas.Application.Ports;
using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;

public class InMemoryUsuarioRepository : IUsuarioRepository
{
    private readonly ConcurrentDictionary<string, Usuario> _usuariosPorEmail = new();
    private readonly ConcurrentDictionary<Guid, Usuario> _usuariosPorId = new();

    public Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _usuariosPorEmail.TryGetValue(email.ToLowerInvariant(), out var usuario);
        return Task.FromResult(usuario);
    }

    public Task<Usuario?> ObtenerPorIdAsync(UsuarioId id, CancellationToken cancellationToken = default)
    {
        _usuariosPorId.TryGetValue(id.Value, out var usuario);
        return Task.FromResult(usuario);
    }

    public Task GuardarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _usuariosPorEmail[usuario.Email.ToLowerInvariant()] = usuario;
        _usuariosPorId[usuario.Id.Value] = usuario;
        return Task.CompletedTask;
    }
}
