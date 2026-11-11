using AssetTracker.API.Data;
using AssetTracker.API.Dto;
using AssetTracker.API.Models;
using AssetTracker.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoRepository _repository;
    private readonly AssetTrackerContext _context;
    private readonly ILogger<EmpleadosController> _logger;

    public EmpleadosController(
        IEmpleadoRepository repository,
        AssetTrackerContext context,
        ILogger<EmpleadosController> logger)
    {
        _repository = repository;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Empleado>>> GetEmpleados()
    {
        return Ok(await _repository.ObtenerTodosAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Empleado>> GetEmpleado(int id)
    {
        var empleado = await _repository.ObtenerPorIdAsync(id);
        if (empleado == null) return NotFound();
        return Ok(empleado);
    }

    [HttpGet("{id}/detalle")]
    public async Task<ActionResult<EmpleadoDetalleDto>> GetEmpleadoDetalle(int id)
    {
        var empleado = await _context.Empleados
            .Include(e => e.Asignaciones)
                .ThenInclude(a => a.Activo)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empleado == null) return NotFound();

        return Ok(new EmpleadoDetalleDto
        {
            Id = empleado.Id,
            NombreCompleto = empleado.NombreCompleto,
            Departamento = empleado.Departamento,
            Documento = empleado.Documento,
            Correo = empleado.Correo,
            Telefono = empleado.Telefono,
            Cargo = empleado.Cargo,
            FechaIngreso = empleado.FechaIngreso,
            Activo = empleado.Activo,
            TotalAsignaciones = empleado.Asignaciones.Count,
            Asignaciones = empleado.Asignaciones
                .OrderByDescending(a => a.FechaAsignacion)
                .Select(a => new AsignacionDetalleDto
                {
                    Id = a.Id,
                    FechaAsignacion = a.FechaAsignacion,
                    ActivoNombre = a.Activo.Nombre,
                    ActivoEstado = a.Activo.Estado,
                    ActivoValor = a.Activo.ValorOriginal
                }).ToList()
        });
    }

    [HttpGet("{id}/tiene-asignaciones")]
    public async Task<ActionResult<object>> EmpleadoTieneAsignaciones(int id)
    {
        var empleado = await _repository.ObtenerPorIdAsync(id);
        if (empleado == null) return NotFound();

        bool tiene = await _context.Asignaciones.AnyAsync(a => a.EmpleadoId == id);
        return Ok(new { empleadoId = id, nombreCompleto = empleado.NombreCompleto, tieneAsignaciones = tiene });
    }

    [HttpPost]
    public async Task<ActionResult<Empleado>> PostEmpleado(CrearEmpleadoDto dto)
    {
        try
        {
            var empleado = new Empleado
            {
                NombreCompleto = dto.NombreCompleto,
                Departamento = dto.Departamento,
                Documento = dto.Documento,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Cargo = dto.Cargo,
                FechaIngreso = dto.FechaIngreso,
                Activo = dto.Activo
            };
            empleado.Validar();
            await _repository.AgregarAsync(empleado);
            _logger.LogInformation("Empleado {Id} creado", empleado.Id);
            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmpleado(int id, CrearEmpleadoDto dto)
    {
        var empleado = await _repository.ObtenerPorIdAsync(id);
        if (empleado == null) return NotFound();

        try
        {
            empleado.NombreCompleto = dto.NombreCompleto;
            empleado.Departamento = dto.Departamento;
            empleado.Documento = dto.Documento;
            empleado.Correo = dto.Correo;
            empleado.Telefono = dto.Telefono;
            empleado.Cargo = dto.Cargo;
            empleado.FechaIngreso = dto.FechaIngreso;
            empleado.Activo = dto.Activo;
            empleado.Validar();
            await _repository.ActualizarAsync(empleado);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmpleado(int id)
    {
        var empleado = await _repository.ObtenerPorIdAsync(id);
        if (empleado == null) return NotFound();

        await _repository.EliminarAsync(empleado);
        return NoContent();
    }
}