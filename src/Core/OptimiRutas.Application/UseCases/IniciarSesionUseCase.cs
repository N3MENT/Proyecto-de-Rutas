namespace OptimiRutas.Application.UseCases;

using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.Ports;

public class IniciarSesionUseCase
{
    private readonly IUsuarioRepository _repository;

    public IniciarSesionUseCase(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<UsuarioDto?> EjecutarAsync(IniciarSesionCommand command, CancellationToken cancellationToken = default)
    {
        var usuario = await _repository.ObtenerPorEmailAsync(command.Email, cancellationToken);
        if (usuario is null)
            return null;

        var hash = RegistrarUsuarioUseCase.HashearContrasena(command.Contrasena);
        if (usuario.ContrasenaHash != hash)
            return null;

        return new UsuarioDto(usuario.Id.Value, usuario.Nombre, usuario.Email);
    }
}
