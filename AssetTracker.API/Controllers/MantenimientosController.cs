using AssetTracker.API.Dto;
using AssetTracker.API.Models;
using AssetTracker.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MantenimientosController : ControllerBase
{
    private readonly IMantenimientoRepository _repository;
    private readonly ILogger<MantenimientosController> _logger;

    public MantenimientosController(IMantenimientoRepository repository, ILogger<MantenimientosController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<MantenimientoResumenDto>>> GetMantenimientos(
        [FromQuery] string? estado = null,
        [FromQuery] string? tipo = null)
    {
        var lista = await _repository.ObtenerTodosAsync();

        var query = lista.AsQueryable();
        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(m => m.ObtenerEstadoReal() == estado);
        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(m => m.Tipo == tipo);

        var resultado = query.Select(m => new MantenimientoResumenDto
        {
            Id = m.Id,
            CodigoInterno = m.CodigoInterno,
            ActivoId = m.ActivoId,
            NombreActivo = m.Activo.Nombre,
            Tipo = m.Tipo,
            FechaProgramada = m.FechaProgramada,
            FechaRealizacion = m.FechaRealizacion,
            Costo = m.Costo,
            EstadoReal = m.ObtenerEstadoReal()
        }).ToList();

        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MantenimientoResumenDto>> GetMantenimiento(int id)
    {
        var m = await _repository.ObtenerPorIdAsync(id);
        if (m == null) return NotFound();

        return Ok(new MantenimientoResumenDto
        {
            Id = m.Id,
            CodigoInterno = m.CodigoInterno,
            ActivoId = m.ActivoId,
            NombreActivo = m.Activo.Nombre,
            Tipo = m.Tipo,
            FechaProgramada = m.FechaProgramada,
            FechaRealizacion = m.FechaRealizacion,
            Costo = m.Costo,
            EstadoReal = m.ObtenerEstadoReal()
        });
    }

    [HttpPost]
    public async Task<ActionResult<Mantenimiento>> PostMantenimiento(CrearMantenimientoDto dto)
    {
        try
        {
            var mantenimiento = new Mantenimiento
            {
                ActivoId = dto.ActivoId,
                Tipo = dto.Tipo,
                Descripcion = dto.Descripcion,
                Proveedor = dto.Proveedor,
                FechaProgramada = dto.FechaProgramada,
                Costo = dto.Costo
            };

            mantenimiento.Validar();
            await _repository.AgregarAsync(mantenimiento);

            _logger.LogInformation("Mantenimiento {Id} creado para activo {ActivoId}", mantenimiento.Id, dto.ActivoId);
            return CreatedAtAction(nameof(GetMantenimiento), new { id = mantenimiento.Id }, mantenimiento);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/estado")]
    public async Task<IActionResult> PutEstado(int id, [FromBody] CambiarEstadoMantenimientoDto dto)
    {
        var m = await _repository.ObtenerPorIdAsync(id);
        if (m == null) return NotFound();

        try
        {
            m.CambiarEstado(dto.Estado);
            await _repository.ActualizarAsync(m);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMantenimiento(int id)
    {
        var m = await _repository.ObtenerPorIdAsync(id);
        if (m == null) return NotFound();

        await _repository.EliminarAsync(m);
        return NoContent();
    }
}

// DTO auxiliar para el cambio de estado
public class CambiarEstadoMantenimientoDto
{
    public string Estado { get; set; } = string.Empty;
}