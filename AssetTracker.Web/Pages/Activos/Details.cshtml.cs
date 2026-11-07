using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public ActivoViewModel? Activo { get; set; }
    public string? Error { get; set; }

    // Estado elegido en el formulario de cambio
    [BindProperty]
    public string NuevoEstado { get; set; } = string.Empty;

    public DetailsModel(IHttpClientFactory factory) => _factory = factory;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        await CargarActivoAsync(id);
        return Activo == null ? NotFound() : Page();
    }

    /// <summary>
    /// Handler nombrado: POST ?handler=CambiarEstado
    /// </summary>
    public async Task<IActionResult> OnPostCambiarEstadoAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            var response = await client.PutAsJsonAsync($"api/activos/{id}/estado", new { Estado = NuevoEstado });

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Details", new { id });
            }

            var detalle = await response.Content.ReadAsStringAsync();
            Error = $"No se pudo cambiar el estado: {detalle}";
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
        }

        await CargarActivoAsync(id);
        return Page();
    }

    private async Task CargarActivoAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            Activo = await client.GetFromJsonAsync<ActivoViewModel>($"api/activos/{id}");

            // El GET por id devuelve la entidad sin discriminator:
            // lo inferimos por las propiedades específicas que viajan en el JSON.
            if (Activo != null && string.IsNullOrEmpty(Activo.Discriminator))
            {
                Activo.Discriminator = Activo.NumeroSerie is not null ? "ActivoFisico" : "ActivoDigital";
            }
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
        }
    }
}