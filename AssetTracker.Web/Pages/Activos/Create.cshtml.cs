using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using AssetTracker.Web.DTOs;

namespace AssetTracker.Web.Pages.Activos;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    [BindProperty]
    public CrearActivoDto Input { get; set; } = new();

    public CreateModel(IHttpClientFactory factory) => _factory = factory;

    public void OnGet()
    {
        Input.TipoActivo = "Fisico";
        Input.FechaAdquisicion = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            var response = await client.PostAsJsonAsync("api/activos", Input);
            if (response.IsSuccessStatusCode) return RedirectToPage("Index");

            var detalle = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"La API respondió {(int)response.StatusCode}: {detalle}");
        }
        catch (HttpRequestException) { ModelState.AddModelError("", "No se pudo conectar con la API."); }

        return Page();
    }
}