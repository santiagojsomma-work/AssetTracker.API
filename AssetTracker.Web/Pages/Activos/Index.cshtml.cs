using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public List<ActivoViewModel> Activos { get; set; } = new();
    public string? Error { get; set; }
    public decimal TotalValor { get; set; }

    // Filtros que viajan por query string (GET)
    [BindProperty(SupportsGet = true)]
    public string? FiltroEstado { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FiltroTexto { get; set; }

    [BindProperty(SupportsGet = true)]
    public string OrdenarPor { get; set; } = "nombre";

    [BindProperty(SupportsGet = true)]
    public bool Descendente { get; set; }

    // Catálogo de estados para el select
    public string[] Estados { get; set; } = Array.Empty<string>();

    public IndexModel(IHttpClientFactory factory) => _factory = factory;

    public async Task OnGetAsync()
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            Estados = await client.GetFromJsonAsync<string[]>("api/activos/estados") ?? Array.Empty<string>();
            string url;

            if (!string.IsNullOrWhiteSpace(FiltroTexto))
            {
                // Búsqueda por texto (nombre, código, serie o licencia)
                url = $"api/activos/buscar?texto={Uri.EscapeDataString(FiltroTexto)}";
            }
            else
            {
                // Listado con filtro por estado + ordenamiento server-side
                url = $"api/activos?ordenarPor={Uri.EscapeDataString(OrdenarPor)}&descendente={Descendente}";
                if (!string.IsNullOrWhiteSpace(FiltroEstado))
                    url += $"&estado={Uri.EscapeDataString(FiltroEstado)}";
            }

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Error = $"La API respondió {(int)response.StatusCode}";
                return;
            }

            var json = await response.Content.ReadAsStringAsync();

            // /api/activos devuelve { datos, totalValor }; /buscar devuelve solo el array
            if (json.TrimStart().StartsWith("["))
            {
                Activos = JsonSerializer.Deserialize<List<ActivoViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                TotalValor = Activos.Sum(a => a.ValorOriginal);
            }
            else
            {
                var resultado = JsonSerializer.Deserialize<RespuestaActivos>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Activos = resultado?.Datos ?? new();
                TotalValor = resultado?.TotalValor ?? 0;
            }
        }
        catch (HttpRequestException) { Error = "No se pudo conectar con la API."; }
        catch (JsonException) { Error = "La API devolvió un formato inesperado."; }
    }

    private class RespuestaActivos
    {
        public List<ActivoViewModel> Datos { get; set; } = new();
        public decimal TotalValor { get; set; }
    }
}