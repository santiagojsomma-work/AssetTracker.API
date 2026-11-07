using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace AssetTracker.Web.Pages.Asignaciones;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _factory;
    public List<AsignacionViewModel> Asignaciones { get; set; } = new();
    public string? Error { get; set; }

    public IndexModel(IHttpClientFactory factory) => _factory = factory;

    public async Task OnGetAsync()
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            // La API devuelve las asignaciones con Activo y Empleado incluidos (gracias al Include del Paso 3)
            Asignaciones = await client.GetFromJsonAsync<List<AsignacionViewModel>>("api/asignaciones") ?? new();
        }
        catch (HttpRequestException) { Error = "No se pudo conectar con la API."; }
    }
}