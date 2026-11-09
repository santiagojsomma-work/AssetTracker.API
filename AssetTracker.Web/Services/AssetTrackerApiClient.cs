using AssetTracker.Web.DTOs;
using AssetTracker.Web.Pages.Empleados;
using AssetTracker.Web.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace AssetTracker.Web.Services;

/// <summary>Resultado de una llamada a la API sin usar excepciones como control de flujo.</summary>
public record ResultadoApi(bool Ok, string? Error = null, int StatusCode = 0);

/// <summary>
/// Punto único de comunicación con AssetTracker.API (reutilización R2).
/// Elimina el try/catch + deserialización duplicados en todos los PageModels.
/// </summary>
public interface IAssetTrackerApiClient
{
    // Catálogos y estadísticas
    Task<(string[] Estados, string? Error)> ObtenerEstadosAsync();
    Task<(EstadisticasViewModel? Estadisticas, string? Error)> ObtenerEstadisticasAsync();
    Task<(List<ReporteEstadoViewModel>? Reporte, string? Error)> ObtenerReportePorEstadoAsync();

    // Activos
    Task<(List<ActivoViewModel>? Activos, decimal TotalValor, string? Error)> ObtenerActivosAsync(string? estado = null, string ordenarPor = "nombre", bool descendente = false);
    Task<(List<ActivoViewModel>? Activos, string? Error)> BuscarActivosAsync(string texto);
    Task<(ActivoViewModel? Activo, string? Error)> ObtenerActivoAsync(int id);
    Task<ResultadoApi> CrearActivoAsync(CrearActivoDto dto);
    Task<ResultadoApi> ActualizarActivoAsync(int id, CrearActivoDto dto);
    Task<ResultadoApi> EliminarActivoAsync(int id);
    Task<ResultadoApi> CambiarEstadoActivoAsync(int id, string estado);

    // Empleados
    Task<(List<EmpleadoViewModel>? Empleados, string? Error)> ObtenerEmpleadosAsync();
    Task<(EmpleadoViewModel? Empleado, string? Error)> ObtenerEmpleadoAsync(int id);
    Task<(EmpleadoDetalleViewModel? Detalle, string? Error)> ObtenerEmpleadoDetalleAsync(int id);
    Task<(bool Tiene, string? Error)> EmpleadoTieneAsignacionesAsync(int id);
    Task<ResultadoApi> CrearEmpleadoAsync(CrearEmpleadoDto dto);
    Task<ResultadoApi> ActualizarEmpleadoAsync(int id, CrearEmpleadoDto dto);
    Task<ResultadoApi> EliminarEmpleadoAsync(int id);

    // Asignaciones
    Task<(List<AsignacionViewModel>? Asignaciones, string? Error)> ObtenerAsignacionesAsync();
    Task<ResultadoApi> CrearAsignacionAsync(CrearAsignacionDto dto);
    Task<ResultadoApi> EliminarAsignacionAsync(int id);
}

public class AssetTrackerApiClient : IAssetTrackerApiClient
{
    private readonly IHttpClientFactory _factory;
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public AssetTrackerApiClient(IHttpClientFactory factory) => _factory = factory;

    private HttpClient Create() => _factory.CreateClient("AssetTrackerApi");

    // ===== Infraestructura privada: el ÚNICO lugar con try/catch y deserialización =====

    private async Task<(T? Datos, string? Error)> GetAsync<T>(string url)
    {
        try
        {
            var response = await Create().GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (default, $"La API respondió {(int)response.StatusCode}");
            var datos = await response.Content.ReadFromJsonAsync<T>(Json);
            return (datos, null);
        }
        catch (HttpRequestException) { return (default, "No se pudo conectar con la API."); }
        catch (JsonException) { return (default, "La API devolvió un formato inesperado."); }
    }

    private async Task<ResultadoApi> SendAsync(HttpMethod method, string url, object? body = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (body != null) request.Content = JsonContent.Create(body);
            var response = await Create().SendAsync(request);
            return response.IsSuccessStatusCode
                ? new ResultadoApi(true, null, (int)response.StatusCode)
                : new ResultadoApi(false, await response.Content.ReadAsStringAsync(), (int)response.StatusCode);
        }
        catch (HttpRequestException) { return new ResultadoApi(false, "No se pudo conectar con la API."); }
    }

    private sealed class Envuelta<T>
    {
        public List<T> Datos { get; set; } = new();
        public decimal TotalValor { get; set; }
    }

    private sealed class TieneAsignacionesRespuesta
    {
        public bool TieneAsignaciones { get; set; }
    }

    // ===== Catálogos y estadísticas =====
    public async Task<(string[] Estados, string? Error)> ObtenerEstadosAsync()
    {
        var (datos, error) = await GetAsync<string[]>("api/activos/estados");
        return (datos ?? Array.Empty<string>(), error);
    }

    public Task<(EstadisticasViewModel? Estadisticas, string? Error)> ObtenerEstadisticasAsync()
        => GetAsync<EstadisticasViewModel>("api/activos/estadisticas");

    public Task<(List<ReporteEstadoViewModel>? Reporte, string? Error)> ObtenerReportePorEstadoAsync()
        => GetAsync<List<ReporteEstadoViewModel>>("api/activos/reporte");

    // ===== Activos =====
    public async Task<(List<ActivoViewModel>? Activos, decimal TotalValor, string? Error)> ObtenerActivosAsync(
        string? estado = null, string ordenarPor = "nombre", bool descendente = false)
    {
        var url = $"api/activos?ordenarPor={Uri.EscapeDataString(ordenarPor)}&descendente={descendente}";
        if (!string.IsNullOrWhiteSpace(estado)) url += $"&estado={Uri.EscapeDataString(estado)}";

        var (env, error) = await GetAsync<Envuelta<ActivoViewModel>>(url);
        return (env?.Datos, env?.TotalValor ?? 0, error);
    }

    public Task<(List<ActivoViewModel>? Activos, string? Error)> BuscarActivosAsync(string texto)
        => GetAsync<List<ActivoViewModel>>($"api/activos/buscar?texto={Uri.EscapeDataString(texto)}");

    public async Task<(ActivoViewModel? Activo, string? Error)> ObtenerActivoAsync(int id)
    {
        var (act, error) = await GetAsync<ActivoViewModel>($"api/activos/{id}");
        // El GET por id devuelve la entidad sin discriminator: se infiere por propiedades del tipo
        if (act != null && string.IsNullOrEmpty(act.Discriminator))
            act.Discriminator = act.NumeroSerie is not null ? "ActivoFisico" : "ActivoDigital";
        return (act, error);
    }

    public Task<ResultadoApi> CrearActivoAsync(CrearActivoDto dto)
        => SendAsync(HttpMethod.Post, "api/activos", dto);
    public Task<ResultadoApi> ActualizarActivoAsync(int id, CrearActivoDto dto)
        => SendAsync(HttpMethod.Put, $"api/activos/{id}", dto);
    public Task<ResultadoApi> EliminarActivoAsync(int id)
        => SendAsync(HttpMethod.Delete, $"api/activos/{id}");
    public Task<ResultadoApi> CambiarEstadoActivoAsync(int id, string estado)
        => SendAsync(HttpMethod.Put, $"api/activos/{id}/estado", new { Estado = estado });

    // ===== Empleados =====
    public Task<(List<EmpleadoViewModel>? Empleados, string? Error)> ObtenerEmpleadosAsync()
        => GetAsync<List<EmpleadoViewModel>>("api/empleados");
    public Task<(EmpleadoViewModel? Empleado, string? Error)> ObtenerEmpleadoAsync(int id)
        => GetAsync<EmpleadoViewModel>($"api/empleados/{id}");
    public Task<(EmpleadoDetalleViewModel? Detalle, string? Error)> ObtenerEmpleadoDetalleAsync(int id)
        => GetAsync<EmpleadoDetalleViewModel>($"api/empleados/{id}/detalle");
    public async Task<(bool Tiene, string? Error)> EmpleadoTieneAsignacionesAsync(int id)
    {
        var (r, error) = await GetAsync<TieneAsignacionesRespuesta>($"api/empleados/{id}/tiene-asignaciones");
        return (r?.TieneAsignaciones ?? false, error);
    }
    public Task<ResultadoApi> CrearEmpleadoAsync(CrearEmpleadoDto dto)
        => SendAsync(HttpMethod.Post, "api/empleados", dto);
    public Task<ResultadoApi> ActualizarEmpleadoAsync(int id, CrearEmpleadoDto dto)
        => SendAsync(HttpMethod.Put, $"api/empleados/{id}", dto);
    public Task<ResultadoApi> EliminarEmpleadoAsync(int id)
        => SendAsync(HttpMethod.Delete, $"api/empleados/{id}");

    // ===== Asignaciones =====
    public Task<(List<AsignacionViewModel>? Asignaciones, string? Error)> ObtenerAsignacionesAsync()
        => GetAsync<List<AsignacionViewModel>>("api/asignaciones");
    public Task<ResultadoApi> CrearAsignacionAsync(CrearAsignacionDto dto)
        => SendAsync(HttpMethod.Post, "api/asignaciones", dto);
    public Task<ResultadoApi> EliminarAsignacionAsync(int id)
        => SendAsync(HttpMethod.Delete, $"api/asignaciones/{id}");
}