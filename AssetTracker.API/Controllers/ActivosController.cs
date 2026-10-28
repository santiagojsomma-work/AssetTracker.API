using AssetTracker.API.Data;
using AssetTracker.API.Dto;
using AssetTracker.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivosController : ControllerBase
{
    private readonly AssetTrackerContext _context;

    public ActivosController(AssetTrackerContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Activo>>> GetActivos()
    {
        return await _context.Activos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Activo>> GetActivo(int id)
    {
        var activo = await _context.Activos.FindAsync(id);
        if (activo == null) return NotFound();
        return activo;
    }

    [HttpPost]
    public async Task<ActionResult<Activo>> PostActivo(CrearActivoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest(new { error = "El nombre es obligatorio." });

        if (dto.ValorOriginal <= 0)
            return BadRequest(new { error = "El valor debe ser mayor a cero." });

        if (dto.FechaAdquisicion > DateTime.Now)
            return BadRequest(new { error = "La fecha no puede ser futura." });

        try
        {
            Activo nuevoActivo = dto.TipoActivo.ToLower() switch
            {
                "fisico" => new ActivoFisico
                {
                    Nombre = dto.Nombre,
                    FechaAdquisicion = dto.FechaAdquisicion,
                    ValorOriginal = dto.ValorOriginal,
                    NumeroSerie = dto.NumeroSerie ?? "",
                    Ubicacion = dto.Ubicacion ?? ""
                },
                "digital" => new ActivoDigital
                {
                    Nombre = dto.Nombre,
                    FechaAdquisicion = dto.FechaAdquisicion,
                    ValorOriginal = dto.ValorOriginal,
                    ClaveLicencia = dto.ClaveLicencia ?? "",
                    FechaVencimientoLicencia = dto.FechaVencimientoLicencia ?? DateTime.MaxValue
                },
                _ => throw new ArgumentException("Tipo inválido. Use 'Fisico' o 'Digital'.")
            };

            nuevoActivo.Validar();
            _context.Activos.Add(nuevoActivo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivo), new { id = nuevoActivo.Id }, nuevoActivo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutActivo(int id, CrearActivoDto dto)
    {
        var activo = await _context.Activos.FindAsync(id);
        if (activo == null) return NotFound();

        activo.Nombre = dto.Nombre;
        activo.FechaAdquisicion = dto.FechaAdquisicion;
        activo.ValorOriginal = dto.ValorOriginal;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivo(int id)
    {
        var activo = await _context.Activos.FindAsync(id);
        if (activo == null) return NotFound();

        _context.Activos.Remove(activo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}