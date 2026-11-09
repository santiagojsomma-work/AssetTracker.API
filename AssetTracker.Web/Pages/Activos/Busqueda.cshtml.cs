using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class BusquedaModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public BusquedaModel(IHttpClientFactory factory) => _factory = factory;

    public void OnGet() { }

    /// <summary>
    /// Proxy de búsqueda en vivo (11B + fix CORS):
    /// el navegador consulta ESTE handler (mismo origen) y el servidor
    /// reenvía la consulta a la API. La URL del backend nunca se expone al cliente.
    /// GET /Activos/Busqueda?handler=Buscar&texto=...
    /// </summary>
    public async Task<IActionResult> OnGetBuscarAsync([FromQuery] string texto = "")
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return new ContentResult { Content = "[]", ContentType = "application/json", StatusCode = 200 };
        }

        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            var response = await client.GetAsync($"api/activos/buscar?texto={Uri.EscapeDataString(texto)}");
            if (!response.IsSuccessStatusCode)
            {
                return new ContentResult { Content = "[]", ContentType = "application/json", StatusCode = 200 };
            }

            // Passthrough: devolvemos el JSON de la API tal cual
            var json = await response.Content.ReadAsStringAsync();
            return new ContentResult { Content = json, ContentType = "application/json", StatusCode = 200 };
        }
        catch (HttpRequestException)
        {
            return new ContentResult { Content = "[]", ContentType = "application/json", StatusCode = 200 };
        }
    }
}