namespace OptimiRutas.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.UseCases;

[ApiController]
[Route("api/[controller]")]
public class RutasController : ControllerBase
{
    private readonly ObtenerTodasLasRutasUseCase _obtenerTodasUseCase;
    private readonly ObtenerRutaPorIdUseCase _obtenerPorIdUseCase;
    private readonly RegistrarRutaUseCase _registrarRutaUseCase;
    private readonly CompletarParadaUseCase _completarParadaUseCase;
    private readonly SugerirRutasUseCase _sugerirRutasUseCase;

    public RutasController(
        ObtenerTodasLasRutasUseCase obtenerTodasUseCase,
        ObtenerRutaPorIdUseCase obtenerPorIdUseCase,
        RegistrarRutaUseCase registrarRutaUseCase,
        CompletarParadaUseCase completarParadaUseCase,
        SugerirRutasUseCase sugerirRutasUseCase)
    {
        _obtenerTodasUseCase = obtenerTodasUseCase;
        _obtenerPorIdUseCase = obtenerPorIdUseCase;
        _registrarRutaUseCase = registrarRutaUseCase;
        _completarParadaUseCase = completarParadaUseCase;
        _sugerirRutasUseCase = sugerirRutasUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RutaDto>>> ObtenerTodas(CancellationToken cancellationToken)
    {
        var dtos = await _obtenerTodasUseCase.EjecutarAsync(cancellationToken);
        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RutaDto>> ObtenerPorId(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _obtenerPorIdUseCase.EjecutarAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(new { mensaje = $"No se encontró la ruta con ID {id}" });

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<RutaDto>> CrearRuta([FromBody] RegistrarRutaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var rutaCreada = await _registrarRutaUseCase.EjecutarAsync(command, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = rutaCreada.Id }, rutaCreada);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("sugerir")]
    public async Task<ActionResult<List<OpcionRutaDto>>> SugerirRutas([FromBody] SugerirRutaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var opciones = await _sugerirRutasUseCase.EjecutarAsync(command, cancellationToken);
            return Ok(opciones);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{rutaId:guid}/paradas/{paradaId:guid}/completar")]
    public async Task<ActionResult<RutaDto>> CompletarParada(Guid rutaId, Guid paradaId, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CompletarParadaCommand(rutaId, paradaId);
            var rutaActualizada = await _completarParadaUseCase.EjecutarAsync(command, cancellationToken);
            return Ok(rutaActualizada);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
