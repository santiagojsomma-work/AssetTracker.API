using AssetTracker.Web.DTOs;
using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Services;

/// <summary>
/// Resultado de una llamada a la API sin usar excepciones como control de flujo.
/// </summary>
public record ResultadoApi(bool Ok, string? Error = null, int StatusCode = 0);

/// <summary>
/// Contrato del punto único de comunicación con AssetTracker.API (R2).
/// Todos los PageModels dependen de esta abstracción, no de HttpClient.
/// </summary>
public interface IAssetTrackerApiClient
{
    // ===== Catálogos y estadísticas =====
    Task<(string[] Estados, string? Error)> ObtenerEstadosAsync();
    Task<(EstadisticasViewModel? Estadisticas, string? Error)> ObtenerEstadisticasAsync();
    Task<(List<ReporteEstadoViewModel>? Reporte, string? Error)> ObtenerReportePorEstadoAsync();

    // ===== Activos =====
    Task<(List<ActivoViewModel>? Activos, decimal TotalValor, string? Error)> ObtenerActivosAsync(
        string? estado = null, string ordenarPor = "nombre", bool descendente = false);
    Task<(List<ActivoViewModel>? Activos, string? Error)> BuscarActivosAsync(string texto);
    Task<(ActivoViewModel? Activo, string? Error)> ObtenerActivoAsync(int id);
    Task<ResultadoApi> CrearActivoAsync(CrearActivoDto dto);
    Task<ResultadoApi> ActualizarActivoAsync(int id, CrearActivoDto dto);
    Task<ResultadoApi> EliminarActivoAsync(int id);
    Task<ResultadoApi> CambiarEstadoActivoAsync(int id, string estado);

    // ===== Empleados =====
    Task<(List<EmpleadoViewModel>? Empleados, string? Error)> ObtenerEmpleadosAsync();
    Task<(EmpleadoViewModel? Empleado, string? Error)> ObtenerEmpleadoAsync(int id);
    Task<(EmpleadoDetalleViewModel? Detalle, string? Error)> ObtenerEmpleadoDetalleAsync(int id);
    Task<(bool Tiene, string? Error)> EmpleadoTieneAsignacionesAsync(int id);
    Task<ResultadoApi> CrearEmpleadoAsync(CrearEmpleadoDto dto);
    Task<ResultadoApi> ActualizarEmpleadoAsync(int id, CrearEmpleadoDto dto);
    Task<ResultadoApi> EliminarEmpleadoAsync(int id);

    // ===== Asignaciones =====
    Task<(List<AsignacionViewModel>? Asignaciones, string? Error)> ObtenerAsignacionesAsync();
    Task<(AsignacionViewModel? Asignacion, string? Error)> ObtenerAsignacionAsync(int id);
    Task<ResultadoApi> CrearAsignacionAsync(CrearAsignacionDto dto);
    Task<ResultadoApi> EliminarAsignacionAsync(int id);
}