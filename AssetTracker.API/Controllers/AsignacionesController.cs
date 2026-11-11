using AssetTracker.API.Dto;
using AssetTracker.API.Models;
using AssetTracker.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AsignacionesController : ControllerBase
{
    private readonly IAsignacionRepository _asignacionRepository;
    private readonly IActivoRepository _activoRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly ILogger<AsignacionesController> _logger;

    public AsignacionesController(
        IAsignacionRepository asignacionRepository,
        IActivoRepository activoRepository,
        IEmpleadoRepository empleadoRepository,
        ILogger<AsignacionesController> logger)
    {
        _asignacionRepository = asignacionRepository;
        _activoRepository = activoRepository;
        _empleadoRepository = empleadoRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Asignacion>>> GetAsignaciones()
    {
        var asignaciones = await _asignacionRepository.ObtenerTodasAsync();
        return Ok(asignaciones);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Asignacion>> GetAsignacion(int id)
    {
        var asignacion = await _asignacionRepository.ObtenerPorIdAsync(id);

        if (asignacion == null)
            return NotFound();

        return Ok(asignacion);
    }

    [HttpPost]
    public async Task<ActionResult<Asignacion>> PostAsignacion(CrearAsignacionDto dto)
    {
        // Verificar que el activo existe
        var activo = await _activoRepository.ObtenerPorIdAsync(dto.ActivoId);
        if (activo == null)
            return BadRequest(new { error = $"El activo con ID {dto.ActivoId} no existe." });

        // Verificar que el empleado existe
        var empleado = await _empleadoRepository.ObtenerPorIdAsync(dto.EmpleadoId);
        if (empleado == null)
            return BadRequest(new { error = $"El empleado con ID {dto.EmpleadoId} no existe." });

        var asignacion = new Asignacion
        {
            ActivoId = dto.ActivoId,
            EmpleadoId = dto.EmpleadoId,
            FechaAsignacion = DateTime.Now
        };

        await _asignacionRepository.AgregarAsync(asignacion);

        // Cambiar estado del activo a "Asignado"
        activo.CambiarEstado("Asignado");
        await _activoRepository.ActualizarAsync(activo);

        _logger.LogInformation("Asignación {AsignacionId} creada: Activo {ActivoId} -> Empleado {EmpleadoId}",
            asignacion.Id, dto.ActivoId, dto.EmpleadoId);

        return CreatedAtAction(nameof(GetAsignacion), new { id = asignacion.Id }, asignacion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsignacion(int id)
    {
        var asignacion = await _asignacionRepository.ObtenerPorIdAsync(id);

        if (asignacion == null)
            return NotFound();

        // Cambiar estado del activo a "Disponible"
        var activo = await _activoRepository.ObtenerPorIdAsync(asignacion.ActivoId);
        if (activo != null)
        {
            activo.CambiarEstado("Disponible");
            await _activoRepository.ActualizarAsync(activo);
        }

        await _asignacionRepository.EliminarAsync(asignacion);

        _logger.LogInformation("Asignación {AsignacionId} eliminada", id);

        return NoContent();
    }
}