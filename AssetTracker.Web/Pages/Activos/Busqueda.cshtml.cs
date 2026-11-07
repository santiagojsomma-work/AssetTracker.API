using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Pages.Activos;

public class BusquedaModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public BusquedaModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Handler con nombre. Se invoca vía AJAX usando ?handler=Buscar
    /// </summary>
    public async Task<JsonResult> OnGetBuscarAsync(string? texto)
    {
        // Si el texto está vacío, devolvemos lista vacía
        if (string.IsNullOrWhiteSpace(texto))
        {
            return new JsonResult(new List<ActivoViewModel>());
        }

        var client = _factory.CreateClient("AssetTrackerApi");

        // Llamamos al endpoint de búsqueda que creamos en el Paso 3 de la API
        var activos = await client.GetFromJsonAsync<List<ActivoViewModel>>(
            $"api/activos/buscar?texto={Uri.EscapeDataString(texto)}");

        return new JsonResult(activos ?? new List<ActivoViewModel>());
    }
}