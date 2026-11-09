using AssetTracker.API.Dto;
using AssetTracker.API.Exceptions;
using AssetTracker.API.Models;
using AssetTracker.API.Repositories;
using AssetTracker.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivosController : ControllerBase
{
    private readonly IActivoRepository _repository;
    private readonly INotificador _notificador;
    private readonly ILogger<ActivosController> _logger;

    public ActivosController(
        IActivoRepository repository,
        INotificador notificador,
        ILogger<ActivosController> logger)
    {
        _repository = repository;
        _notificador = notificador;
        _logger = logger;
    }

    // =====================================================================
    // GET /api/activos
    // Sin filtro de estado: usa ObtenerTodosAsync (camino simple).
    // Con filtro: construye la consulta SQL vía Consultar().
    // =====================================================================
    [HttpGet]
    public async Task<ActionResult<object>> GetActivos(
        [FromQuery] string? estado = null,
        [FromQuery] string ordenarPor = "nombre",
        [FromQuery] bool descendente = false)
    {
        _logger.LogInformation("Consultando activos con filtro {Estado}", estado ?? "todos");

        IQueryable<Activo> query = string.IsNullOrWhiteSpace(estado)
            ? (await _repository.ObtenerTodosAsync()).AsQueryable()
            : _repository.Consultar().Where(a => a.Estado == estado);

        string criterio = ordenarPor?.ToLower() ?? "nombre";
        query = criterio switch
        {
            "valor" => descendente
                ? query.OrderByDescending(a => a.ValorOriginal)
                : query.OrderBy(a => a.ValorOriginal),
            "fecha" => descendente
                ? query.OrderByDescending(a => a.FechaAdquisicion)
                : query.OrderBy(a => a.FechaAdquisicion),
            _ => descendente
                ? query.OrderByDescending(a => a.Nombre)
                : query.OrderBy(a => a.Nombre)
        };

        // Proyección reutilizada (R1)
        var activos = query.Select(Proyecciones.Resumen).ToList();
        decimal totalValor = activos.Sum(a => a.ValorOriginal);

        return Ok(new { datos = activos, totalValor });
    }

    // =====================================================================
    // GET /api/activos/{id}
    // =====================================================================
    [HttpGet("{id}")]
    public async Task<ActionResult<Activo>> GetActivo(int id)
    {
        var activo = await _repository.ObtenerPorIdAsync(id);
        if (activo == null)
        {
            _logger.LogWarning("Activo {ActivoId} no encontrado", id);
            return NotFound();
        }
        return Ok(activo);
    }

    // =====================================================================
    // GET /api/activos/buscar?texto=...
    // Búsqueda por nombre, código, serie o licencia (EF.Functions.Like)
    // =====================================================================
    [HttpGet("buscar")]
    public async Task<ActionResult<List<ActivoResumenDto>>> BuscarActivos(
        [FromQuery] string texto = "")
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return BadRequest(new { error = "El texto de búsqueda es obligatorio." });
        }

        _logger.LogInformation("Buscando activos con texto: {Texto}", texto);

        var activos = await _repository.Consultar()
            .Where(a => EF.Functions.Like(a.Nombre, $"%{texto}%") ||
                        EF.Functions.Like(a.CodigoInterno, $"%{texto}%") ||
                        (a is ActivoFisico && EF.Functions.Like(((ActivoFisico)a).NumeroSerie, $"%{texto}%")) ||
                        (a is ActivoDigital && EF.Functions.Like(((ActivoDigital)a).ClaveLicencia, $"%{texto}%")))
            .OrderBy(a => a.Nombre)
            .Select(Proyecciones.Resumen)
            .ToListAsync();

        return Ok(activos);
    }

    // =====================================================================
    // GET /api/activos/reporte
    // GroupBy por estado
    // =====================================================================
    [HttpGet("reporte")]
    public async Task<ActionResult<List<ReporteActivoDto>>> GetReporteActivos()
    {
        _logger.LogInformation("Generando reporte de activos agrupados por estado");

        var reporte = await _repository.Consultar()
            .GroupBy(a => a.Estado)
            .Select(g => new ReporteActivoDto
            {
                Estado = g.Key,
                Cantidad = g.Count(),
                ValorTotal = g.Sum(a => a.ValorOriginal),
                ValorPromedio = g.Average(a => a.ValorOriginal),
                ValorMaximo = g.Max(a => a.ValorOriginal),
                ValorMinimo = g.Min(a => a.ValorOriginal)
            })
            .OrderByDescending(r => r.Cantidad)
            .ToListAsync();

        return Ok(reporte);
    }

    // =====================================================================
    // GET /api/activos/estadisticas
    // Agregaciones SQL: Count, Sum, Max, Min, Average, Any, Distinct
    // =====================================================================
    [HttpGet("estadisticas")]
    public async Task<ActionResult<object>> GetEstadisticasActivos()
    {
        _logger.LogInformation("Calculando estadísticas generales de activos");

        var query = _repository.Consultar();

        int totalActivos = await query.CountAsync();
        decimal valorTotal = await query.SumAsync(a => (decimal?)a.ValorOriginal) ?? 0m;
        decimal valorPromedio = await query.AverageAsync(a => (decimal?)a.ValorOriginal) ?? 0m;
        decimal valorMaximo = await query.MaxAsync(a => (decimal?)a.ValorOriginal) ?? 0m;
        decimal valorMinimo = await query.MinAsync(a => (decimal?)a.ValorOriginal) ?? 0m;
        bool hayActivosDisponibles = await query.AnyAsync(a => a.Estado == EstadosActivo.Disponible);
        bool hayActivosAsignados = await query.AnyAsync(a => a.Estado == EstadosActivo.Asignado);
        int activosDisponibles = await query.CountAsync(a => a.Estado == EstadosActivo.Disponible);
        int activosAsignados = await query.CountAsync(a => a.Estado == EstadosActivo.Asignado);
        int estadosUnicos = await query.Select(a => a.Estado).Distinct().CountAsync();

        return Ok(new
        {
            totalActivos,
            valorTotal,
            valorPromedio,
            valorMaximo,
            valorMinimo,
            activosDisponibles,
            activosAsignados,
            hayActivosDisponibles,
            hayActivosAsignados,
            estadosUnicos
        });
    }

    // =====================================================================
    // POST /api/activos
    // =====================================================================
    [HttpPost]
    public async Task<ActionResult<Activo>> PostActivo(CrearActivoDto dto)
    {
        try
        {
            string tipo = dto.TipoActivo?.ToLower() ?? string.Empty;

            Activo nuevoActivo = tipo switch
            {
                "fisico" => new ActivoFisico
                {
                    Nombre = dto.Nombre,
                    FechaAdquisicion = dto.FechaAdquisicion,
                    ValorOriginal = dto.ValorOriginal,
                    NumeroSerie = dto.NumeroSerie ?? string.Empty,
                    Ubicacion = dto.Ubicacion ?? string.Empty
                },
                "digital" => new ActivoDigital
                {
                    Nombre = dto.Nombre,
                    FechaAdquisicion = dto.FechaAdquisicion,
                    ValorOriginal = dto.ValorOriginal,
                    ClaveLicencia = dto.ClaveLicencia ?? string.Empty,
                    FechaVencimientoLicencia = dto.FechaVencimientoLicencia ?? DateTime.MaxValue
                },
                _ => throw new ArgumentException("Tipo inválido. Use 'Fisico' o 'Digital'.")
            };

            nuevoActivo.Validar();
            await _repository.AgregarAsync(nuevoActivo);
            await _notificador.NotificarAsync($"Se ha registrado un nuevo activo: {nuevoActivo.Nombre}");
            _logger.LogInformation("Activo {ActivoId} creado exitosamente", nuevoActivo.Id);

            return CreatedAtAction(nameof(GetActivo), new { id = nuevoActivo.Id }, nuevoActivo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error de validación al crear activo: {Mensaje}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    // =====================================================================
    // PUT /api/activos/{id}
    // =====================================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> PutActivo(int id, CrearActivoDto dto)
    {
        var activo = await _repository.ObtenerPorIdAsync(id);
        if (activo == null)
        {
            _logger.LogWarning("Intento de actualizar activo inexistente {ActivoId}", id);
            return NotFound();
        }

        try
        {
            string tipoDto = dto.TipoActivo?.ToLower() ?? string.Empty;
            bool esFisico = activo is ActivoFisico;

            if ((tipoDto == "fisico" && !esFisico) || (tipoDto == "digital" && esFisico))
            {
                return BadRequest(new
                {
                    error = $"No se puede cambiar el tipo de activo. El activo existente es de tipo '{(esFisico ? "fisico" : "digital")}'."
                });
            }

            activo.Nombre = dto.Nombre;
            activo.FechaAdquisicion = dto.FechaAdquisicion;
            activo.ValorOriginal = dto.ValorOriginal;

            if (activo is ActivoFisico fisico)
            {
                fisico.NumeroSerie = dto.NumeroSerie ?? string.Empty;
                fisico.Ubicacion = dto.Ubicacion ?? string.Empty;
            }
            else if (activo is ActivoDigital digital)
            {
                digital.ClaveLicencia = dto.ClaveLicencia ?? string.Empty;
                digital.FechaVencimientoLicencia = dto.FechaVencimientoLicencia ?? DateTime.MaxValue;
            }

            activo.Validar();
            await _repository.ActualizarAsync(activo);
            _logger.LogInformation("Activo {ActivoId} actualizado", id);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error de validación al actualizar activo {ActivoId}: {Mensaje}", id, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    // =====================================================================
    // PUT /api/activos/{id}/estado
    // Regla de negocio: Disponible/Asignado solo los gestiona Asignaciones.
    // =====================================================================
    [HttpPut("{id}/estado")]
    public async Task<IActionResult> PutEstado(int id, CambiarEstadoDto dto)
    {
        var activo = await _repository.ObtenerPorIdAsync(id);
        if (activo == null)
        {
            _logger.LogWarning("Intento de cambiar estado de activo inexistente {ActivoId}", id);
            return NotFound();
        }

        if (dto.Estado == EstadosActivo.Disponible || dto.Estado == EstadosActivo.Asignado)
        {
            return BadRequest(new
            {
                error = "Los estados 'Disponible' y 'Asignado' se gestionan automáticamente desde el módulo de Asignaciones."
            });
        }

        try
        {
            activo.CambiarEstado(dto.Estado);
            await _repository.ActualizarAsync(activo);
            _logger.LogInformation("Activo {ActivoId} cambió a estado {Estado}", id, dto.Estado);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Cambio de estado inválido para activo {ActivoId}: {Mensaje}", id, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    // =====================================================================
    // DELETE /api/activos/{id}
    // 409 con la cantidad REAL de asignaciones (fix S7).
    // =====================================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivo(int id)
    {
        var activo = await _repository.ObtenerPorIdAsync(id);
        if (activo == null)
            return NotFound();

        try
        {
            int cantidadAsignaciones = await _repository.ContarAsignacionesAsync(id);
            if (cantidadAsignaciones > 0)
            {
                throw new ActivoConDependenciasException(id, cantidadAsignaciones);
            }

            await _repository.EliminarAsync(activo);
            _logger.LogInformation("Activo {ActivoId} eliminado", id);
            return NoContent();
        }
        catch (ActivoConDependenciasException ex)
        {
            _logger.LogWarning("Activo {ActivoId} no se puede eliminar: {Mensaje}", id, ex.Message);
            return Conflict(new
            {
                error = ex.Message,
                activoId = ex.ActivoId,
                cantidadAsignaciones = ex.CantidadAsignaciones
            });
        }
    }
}