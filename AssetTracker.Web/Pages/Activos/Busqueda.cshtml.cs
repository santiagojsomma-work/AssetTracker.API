using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class BusquedaModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    // Mismo contrato que la API: camelCase (JsonSerializerDefaults.Web)
    private static readonly JsonSerializerOptions JsonWeb = new(JsonSerializerDefaults.Web);

    public BusquedaModel(IAssetTrackerApiClient api) => _api = api;

    public void OnGet() { }

    /// <summary>
    /// Proxy same-origin de la búsqueda en vivo (evita CORS y oculta el backend).
    /// GET /Activos/Busqueda?handler=Buscar&texto=...
    /// </summary>
    public async Task<IActionResult> OnGetBuscarAsync([FromQuery] string texto = "")
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return new ContentResult { Content = "[]", ContentType = "application/json", StatusCode = 200 };
        }

        var (activos, _) = await _api.BuscarActivosAsync(texto);

        return new ContentResult
        {
            Content = JsonSerializer.Serialize(activos ?? new(), JsonWeb),
            ContentType = "application/json",
            StatusCode = 200
        };
    }
}