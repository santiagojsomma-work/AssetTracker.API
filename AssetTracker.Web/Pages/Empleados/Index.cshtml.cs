using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _factory;
    public List<EmpleadoViewModel> Empleados { get; set; } = new();
    public string? Error { get; set; }

    public IndexModel(IHttpClientFactory factory) => _factory = factory;

    public async Task OnGetAsync()
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            Empleados = await client.GetFromJsonAsync<List<EmpleadoViewModel>>("api/empleados") ?? new();
        }
        catch (HttpRequestException) { Error = "No se pudo conectar con la API."; }
    }
}