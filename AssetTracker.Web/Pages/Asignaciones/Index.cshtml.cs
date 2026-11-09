using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

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
            var response = await client.GetAsync("api/asignaciones");
            if (!response.IsSuccessStatusCode)
            {
                Error = $"La API respondió {(int)response.StatusCode}";
                return;
            }

            Asignaciones = await response.Content.ReadFromJsonAsync<List<AsignacionViewModel>>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }
        catch (HttpRequestException) { Error = "No se pudo conectar con la API."; }
    }
}