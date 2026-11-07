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

    /// <summary>
    /// GET /api/empleados
    /// Lista todos los empleados.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Empleado>>> GetEmpleados()
    {
        _logger.LogInformation("Consultando todos los empleados");

        var empleados = await _repository.ObtenerTodosAsync();
        return Ok(empleados);
    }

    /// <summary>
    /// GET /api/empleados/1
    /// Obtiene un empleado por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Empleado>> GetEmpleado(int id)
    {
        var empleado = await _repository.ObtenerPorIdAsync(id);

        if (empleado == null)
        {
            _logger.LogWarning("Empleado {EmpleadoId} no encontrado", id);
            return NotFound();
        }

        return Ok(empleado);
    }

    /// <summary>
    /// GET /api/empleados/1/detalle
    /// Obtiene un empleado con sus asignaciones y los activos asociados.
    /// Usa Include y ThenInclude para cargar las relaciones en una sola consulta SQL.
    /// </summary>
    [HttpGet("{id}/detalle")]
    public async Task<ActionResult<EmpleadoDetalleDto>> GetEmpleadoDetalle(int id)
    {
        _logger.LogInformation("Consultando detalle del empleado {EmpleadoId}", id);

        // Include carga las asignaciones del empleado.
        // ThenInclude carga el activo asociado a cada asignación.
        // Esto se traduce a un SQL con JOINs, evitando el problema N+1.
        var empleado = await _context.Empleados
            .Include(e => e.Asignaciones)
                .ThenInclude(a => a.Activo)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empleado == null)
        {
            _logger.LogWarning("Empleado {EmpleadoId} no encontrado para detalle", id);
            return NotFound();
        }

        // Proyectamos a DTO para evitar ciclos de serialización
        // y exponer solo los datos necesarios.
        var dto = new EmpleadoDetalleDto
        {
            Id = empleado.Id,
            NombreCompleto = empleado.NombreCompleto,
            Departamento = empleado.Departamento,
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
                })
                .ToList()
        };

        return Ok(dto);
    }

    /// <summary>
    /// GET /api/empleados/1/tiene-asignaciones
    /// Verifica si un empleado tiene asignaciones activas usando Any.
    /// </summary>
    [HttpGet("{id}/tiene-asignaciones")]
    public async Task<ActionResult<object>> EmpleadoTieneAsignaciones(int id)
    {
        var empleado = await _repository.ObtenerPorIdAsync(id);

        if (empleado == null)
        {
            _logger.LogWarning("Empleado {EmpleadoId} no encontrado", id);
            return NotFound();
        }

        // Any genera un SQL EXISTS, más eficiente que Count > 0
        // porque se detiene en el primer registro encontrado.
        bool tieneAsignaciones = await _context.Asignaciones
            .AnyAsync(a => a.EmpleadoId == id);

        return Ok(new
        {
            empleadoId = id,
            nombreCompleto = empleado.NombreCompleto,
            tieneAsignaciones
        });
    }

    /// <summary>
    /// POST /api/empleados
    /// Crea un nuevo empleado.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Empleado>> PostEmpleado(CrearEmpleadoDto dto)
    {
        var empleado = new Empleado
        {
            NombreCompleto = dto.NombreCompleto,
            Departamento = dto.Departamento
        };

        await _repository.AgregarAsync(empleado);

        _logger.LogInformation("Empleado {EmpleadoId} creado", empleado.Id);

        return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
    }

    /// <summary>
    /// PUT /api/empleados/1
    /// Actualiza un empleado existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmpleado(int id, CrearEmpleadoDto dto)
    {
        var empleado = await _repository.ObtenerPorIdAsync(id);

        if (empleado == null)
        {
            _logger.LogWarning("Intento de actualizar empleado inexistente {EmpleadoId}", id);
            return NotFound();
        }

        empleado.NombreCompleto = dto.NombreCompleto;
        empleado.Departamento = dto.Departamento;

        await _repository.ActualizarAsync(empleado);

        _logger.LogInformation("Empleado {EmpleadoId} actualizado", id);

        return NoContent();
    }

    /// <summary>
    /// DELETE /api/empleados/1
    /// Elimina un empleado. Por configuración en el DbContext,
    /// sus asignaciones se eliminan en cascada.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmpleado(int id)
    {
        var empleado = await _repository.ObtenerPorIdAsync(id);

        if (empleado == null)
        {
            _logger.LogWarning("Intento de eliminar empleado inexistente {EmpleadoId}", id);
            return NotFound();
        }

        await _repository.EliminarAsync(empleado);

        _logger.LogInformation("Empleado {EmpleadoId} eliminado", id);

        return NoContent();
    }
}