using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using AssetTracker.Web.DTOs;
using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Pages.Empleados;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    [BindProperty]
    public CrearEmpleadoDto Input { get; set; } = new();

    public EditModel(IHttpClientFactory factory) => _factory = factory;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            var empleado = await client.GetFromJsonAsync<EmpleadoViewModel>($"api/empleados/{id}");

            if (empleado == null)
                return NotFound();

            // Mapear el ViewModel al DTO de edición
            Input = new CrearEmpleadoDto
            {
                NombreCompleto = empleado.NombreCompleto,
                Departamento = empleado.Departamento
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
            var response = await client.PutAsJsonAsync($"api/empleados/{id}", Input);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            var detalle = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"La API respondió {(int)response.StatusCode}: {detalle}");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "No se pudo conectar con la API.");
        }

        return Page();
    }
}