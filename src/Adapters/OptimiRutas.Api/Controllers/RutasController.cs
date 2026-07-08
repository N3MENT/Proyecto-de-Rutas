namespace OptimiRutas.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OptimiRutas.Application.DTOs;
using OptimiRutas.Application.Ports;
using OptimiRutas.Application.UseCases;
using OptimiRutas.Domain.ValueObjects;

[ApiController]
[Route("api/[controller]")]
public class RutasController : ControllerBase
{
    private readonly RegistrarRutaUseCase _registrarRutaUseCase;
    private readonly CompletarParadaUseCase _completarParadaUseCase;
    private readonly IRutaRepository _repository;

    public RutasController(
        RegistrarRutaUseCase registrarRutaUseCase,
        CompletarParadaUseCase completarParadaUseCase,
        IRutaRepository repository)
    {
        _registrarRutaUseCase = registrarRutaUseCase;
        _completarParadaUseCase = completarParadaUseCase;
        _repository = repository;
    }

    // GET: api/rutas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RutaDto>>> ObtenerTodas(CancellationToken cancellationToken)
    {
        var rutas = await _repository.ObtenerTodasAsync(cancellationToken);
        var dtos = rutas.Select(RegistrarRutaUseCase.MapearARutaDto);
        return Ok(dtos);
    }

    // GET: api/rutas/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RutaDto>> ObtenerPorId(Guid id, CancellationToken cancellationToken)
    {
        var ruta = await _repository.ObtenerPorIdAsync(new RutaId(id), cancellationToken);
        if (ruta is null)
            return NotFound(new { mensaje = $"No se encontró la ruta con ID {id}" });

        return Ok(RegistrarRutaUseCase.MapearARutaDto(ruta));
    }

    // POST: api/rutas
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

    // PATCH: api/rutas/{rutaId}/paradas/{paradaId}/completar
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