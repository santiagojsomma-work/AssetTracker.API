using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Pages.Empleados;

public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _factory;
    public EmpleadoViewModel? Empleado { get; set; }
    public string? Error { get; set; }

    public DeleteModel(IHttpClientFactory factory) => _factory = factory;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            var response = await client.GetAsync($"api/empleados/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                Error = $"La API respondió {(int)response.StatusCode}";
                return Page();
            }

            Empleado = await response.Content.ReadFromJsonAsync<EmpleadoViewModel>();
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
            var response = await client.DeleteAsync($"api/empleados/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            // Si el empleado tiene asignaciones, la API devuelve 204 (cascade) 
            // pero si hubiera conflicto lo manejamos aquí
            var detalle = await response.Content.ReadAsStringAsync();
            Error = $"La API respondió {(int)response.StatusCode}: {detalle}";
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
        }

        // Recargar el empleado para mostrar la confirmación nuevamente
        if (Empleado == null)
        {
            try
            {
                Empleado = await (await client.GetAsync($"api/empleados/{id}")).Content
                    .ReadFromJsonAsync<EmpleadoViewModel>();
            }
            catch { }
        }

        return Page();
    }
}