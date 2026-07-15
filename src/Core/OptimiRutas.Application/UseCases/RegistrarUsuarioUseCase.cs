namespace OptimiRutas.Application.UseCases;

using System.Security.Cryptography;
using System.Text;
using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.Ports;
using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;

public class RegistrarUsuarioUseCase
{
    private readonly IUsuarioRepository _repository;

    public RegistrarUsuarioUseCase(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<UsuarioDto> EjecutarAsync(RegistrarUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        var existente = await _repository.ObtenerPorEmailAsync(command.Email, cancellationToken);
        if (existente is not null)
            throw new InvalidOperationException("Ya existe una cuenta con este correo electrónico.");

        var id = UsuarioId.Nuevo();
        var hash = HashearContrasena(command.Contrasena);
        var usuario = new Usuario(id, command.Nombre, command.Email, hash);

        await _repository.GuardarAsync(usuario, cancellationToken);

        return new UsuarioDto(id.Value, usuario.Nombre, usuario.Email);
    }

    internal static string HashearContrasena(string contrasena)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
        return Convert.ToBase64String(bytes);
    }
}
