using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using AssetTracker.Web.DTOs;
using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Pages.Activos;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    [BindProperty]
    public CrearActivoDto Input { get; set; } = new();

    public EditModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            var activo = await client.GetFromJsonAsync<ActivoViewModel>($"api/activos/{id}");

            if (activo == null)
                return NotFound();

            // Mapear el ViewModel al DTO de edición
            Input = new CrearActivoDto
            {
                Nombre = activo.Nombre,
                FechaAdquisicion = activo.FechaAdquisicion,
                ValorOriginal = activo.ValorOriginal,
                TipoActivo = activo.Discriminator == "ActivoFisico" ? "Fisico" : "Digital",
                NumeroSerie = activo.NumeroSerie,
                Ubicacion = activo.Ubicacion,
                ClaveLicencia = activo.ClaveLicencia,
                FechaVencimientoLicencia = activo.FechaVencimientoLicencia
            };

            return Page();
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "No se pudo conectar con la API.");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            var respuesta = await client.PutAsJsonAsync($"api/activos/{id}", Input);

            if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (respuesta.IsSuccessStatusCode)
                return RedirectToPage("Index");

            var detalle = await respuesta.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"La API respondió {(int)respuesta.StatusCode}: {detalle}");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "No se pudo conectar con la API.");
        }

        return Page();
    }
}