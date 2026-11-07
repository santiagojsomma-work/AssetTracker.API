using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Pages.Activos;

[Authorize(Roles = "Administrador")] 
public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _factory;
    public ActivoViewModel? Activo { get; set; }
    public string? Error { get; set; }

    public DeleteModel(IHttpClientFactory factory) => _factory = factory;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            var response = await client.GetAsync($"api/activos/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                Error = $"La API respondió {(int)response.StatusCode}";
                return Page();
            }

            Activo = await response.Content.ReadFromJsonAsync<ActivoViewModel>();
            return Page();
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            var response = await client.DeleteAsync($"api/activos/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            var detalle = await response.Content.ReadAsStringAsync();
            Error = $"La API respondió {(int)response.StatusCode}: {detalle}";
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
        }

        if (Activo == null)
        {
            try
            {
                Activo = await (await client.GetAsync($"api/activos/{id}")).Content
                    .ReadFromJsonAsync<ActivoViewModel>();
            }
            catch { }
        }

        return Page();
    }
}