using AssetTracker.API.Data;
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
    private readonly AssetTrackerContext _context;
    private readonly INotificador _notificador;
    private readonly ILogger<ActivosController> _logger;

    public ActivosController(
        IActivoRepository repository,
        AssetTrackerContext context,
        INotificador notificador,
        ILogger<ActivosController> logger)
    {
        _repository = repository;
        _context = context;
        _notificador = notificador;
        _logger = logger;
    }

    // =====================================================================
    // GET /api/activos
    // Listado con filtros, ordenamiento y agregación
    // =====================================================================
    [HttpGet]
    public async Task<ActionResult<object>> GetActivos(
        [FromQuery] string? estado = null,
        [FromQuery] string ordenarPor = "nombre",
        [FromQuery] bool descendente = false)
    {
        _logger.LogInformation("Consultando activos con filtro {Estado}", estado ?? "todos");

        IQueryable<Activo> query = _context.Activos;

        // Where: filtra por estado si el cliente envía el parámetro
        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(a => a.Estado == estado);
        }

        // Sum: agregación sobre el conjunto filtrado
        decimal totalValor = await query.SumAsync(a => (decimal?)a.ValorOriginal) ?? 0m;

        // OrderBy / OrderByDescending según parámetro
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

        // Select + ToListAsync: proyección a DTO y ejecución
        var activos = await query
            .Select(a => new ActivoResumenDto
            {
                Id = a.Id,
                CodigoInterno = a.CodigoInterno,                                       // FASE A
                Nombre = a.Nombre,
                FechaAdquisicion = a.FechaAdquisicion,
                ValorOriginal = a.ValorOriginal,
                Estado = a.Estado,
                Discriminator = a is ActivoFisico ? "ActivoFisico" : "ActivoDigital"   // FASE C
            })
            .ToListAsync();

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
    // GET /api/activos/buscar?texto=dell
    // Búsqueda con EF.Functions.Like (LINQ + SQL LIKE)
    // Incluye nombre, código interno, número de serie y clave de licencia
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

        // EF.Functions.Like traduce a SQL LIKE
        var activos = await _context.Activos
            .Where(a => EF.Functions.Like(a.Nombre, $"%{texto}%") ||
                        EF.Functions.Like(a.CodigoInterno, $"%{texto}%") ||              // FASE A
                        (a is ActivoFisico && EF.Functions.Like(((ActivoFisico)a).NumeroSerie, $"%{texto}%")) ||
                        (a is ActivoDigital && EF.Functions.Like(((ActivoDigital)a).ClaveLicencia, $"%{texto}%")))
            .OrderBy(a => a.Nombre)
            .Select(a => new ActivoResumenDto
            {
                Id = a.Id,
                CodigoInterno = a.CodigoInterno,                                       // FASE A
                Nombre = a.Nombre,
                FechaAdquisicion = a.FechaAdquisicion,
                ValorOriginal = a.ValorOriginal,
                Estado = a.Estado,
                Discriminator = a is ActivoFisico ? "ActivoFisico" : "ActivoDigital"   // FASE C
            })
            .ToListAsync();

        return Ok(activos);
    }

    // =====================================================================
    // GET /api/activos/paginado?pagina=1&tamanio=10
    // Paginación con Skip y Take
    // =====================================================================
    [HttpGet("paginado")]
    public async Task<ActionResult<object>> GetActivosPaginados(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanio = 10)
    {
        if (pagina < 1) pagina = 1;
        if (tamanio < 1 || tamanio > 100) tamanio = 10;

        _logger.LogInformation("Consultando activos paginados: página {Pagina}, tamaño {Tamanio}", pagina, tamanio);

        // Count: total de registros
        int totalRegistros = await _context.Activos.CountAsync();

        // Skip: cuántos registros saltar
        int saltar = (pagina - 1) * tamanio;

        // Skip + Take: paginación
        var activos = await _context.Activos
            .OrderBy(a => a.Id)
            .Skip(saltar)
            .Take(tamanio)
            .Select(a => new ActivoResumenDto
            {
                Id = a.Id,
                CodigoInterno = a.CodigoInterno,                                       // FASE A
                Nombre = a.Nombre,
                FechaAdquisicion = a.FechaAdquisicion,
                ValorOriginal = a.ValorOriginal,
                Estado = a.Estado,
                Discriminator = a is ActivoFisico ? "ActivoFisico" : "ActivoDigital"   // FASE C
            })
            .ToListAsync();

        int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanio);

        return Ok(new
        {
            datos = activos,
            paginacion = new
            {
                paginaActual = pagina,
                tamanioPagina = tamanio,
                totalRegistros,
                totalPaginas,
                tienePaginaSiguiente = pagina < totalPaginas,
                tienePaginaAnterior = pagina > 1
            }
        });
    }

    // =====================================================================
    // GET /api/activos/reporte
    // Reporte agrupado por estado usando GroupBy
    // =====================================================================
    [HttpGet("reporte")]
    public async Task<ActionResult<List<ReporteActivoDto>>> GetReporteActivos()
    {
        _logger.LogInformation("Generando reporte de activos agrupados por estado");

        var reporte = await _context.Activos
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
    // Estadísticas generales con Count, Sum, Max, Min, Average, Any, Distinct
    // =====================================================================
    [HttpGet("estadisticas")]
    public async Task<ActionResult<object>> GetEstadisticasActivos()
    {
        _logger.LogInformation("Calculando estadísticas generales de activos");

        int totalActivos = await _context.Activos.CountAsync();
        decimal valorTotal = await _context.Activos.SumAsync(a => (decimal?)a.ValorOriginal) ?? 0m;
        decimal valorPromedio = await _context.Activos.AverageAsync(a => (decimal?)a.ValorOriginal) ?? 0m;
        decimal valorMaximo = await _context.Activos.MaxAsync(a => (decimal?)a.ValorOriginal) ?? 0m;
        decimal valorMinimo = await _context.Activos.MinAsync(a => (decimal?)a.ValorOriginal) ?? 0m;
        bool hayActivosDisponibles = await _context.Activos.AnyAsync(a => a.Estado == "Disponible");
        bool hayActivosAsignados = await _context.Activos.AnyAsync(a => a.Estado == "Asignado");
        int activosDisponibles = await _context.Activos.CountAsync(a => a.Estado == "Disponible");
        int activosAsignados = await _context.Activos.CountAsync(a => a.Estado == "Asignado");
        int estadosUnicos = await _context.Activos.Select(a => a.Estado).Distinct().CountAsync();

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
    // Crear un activo (físico o digital). El código interno lo genera el repositorio.
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
    // Actualizar un activo existente (no permite cambiar el tipo)
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
            string tipoReal = activo switch
            {
                ActivoFisico => "fisico",
                ActivoDigital => "digital",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(tipoDto) && tipoDto != tipoReal)
            {
                return BadRequest(new
                {
                    error = $"No se puede cambiar el tipo de activo. El activo existente es de tipo '{tipoReal}'."
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
    // FASE C: cambio manual de estado con regla de negocio:
    // "Disponible" y "Asignado" los gestiona SOLO el módulo de Asignaciones.
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
    // Eliminar un activo. Devuelve 409 si tiene asignaciones asociadas.
    // =====================================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivo(int id)
    {
        var activo = await _repository.ObtenerPorIdAsync(id);
        if (activo == null)
            return NotFound();

        try
        {
            bool tieneAsignaciones = await _repository.TieneAsignacionesAsync(id);
            if (tieneAsignaciones)
            {
                throw new ActivoConDependenciasException(id, 1);
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