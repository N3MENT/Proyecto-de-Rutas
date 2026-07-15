namespace OptimiRutas.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.UseCases;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegistrarUsuarioUseCase _registrarUseCase;
    private readonly IniciarSesionUseCase _iniciarSesionUseCase;

    public AuthController(
        RegistrarUsuarioUseCase registrarUseCase,
        IniciarSesionUseCase iniciarSesionUseCase)
    {
        _registrarUseCase = registrarUseCase;
        _iniciarSesionUseCase = iniciarSesionUseCase;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<UsuarioDto>> Registrar([FromBody] RegistrarUsuarioCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _registrarUseCase.EjecutarAsync(command, cancellationToken);
            return Created("", usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UsuarioDto>> Login([FromBody] IniciarSesionCommand command, CancellationToken cancellationToken)
    {
        var usuario = await _iniciarSesionUseCase.EjecutarAsync(command, cancellationToken);
        if (usuario is null)
            return Unauthorized(new { error = "Correo o contraseña incorrectos." });

        return Ok(usuario);
    }
}
