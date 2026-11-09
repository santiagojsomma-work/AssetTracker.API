using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize(Roles = "Administrador")]
public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public EmpleadoDeleteViewModel? Empleado { get; set; }
    public string? Error { get; set; }

    public DeleteModel(IHttpClientFactory factory) => _factory = factory;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        await CargarEmpleadoAsync(id);
        return Empleado == null ? NotFound() : Page();
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

            var detalle = await response.Content.ReadAsStringAsync();
            Error = $"La API respondió {(int)response.StatusCode}: {detalle}";
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
        }

        await CargarEmpleadoAsync(id);
        return Page();
    }

    private async Task CargarEmpleadoAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            // 1. Obtener datos básicos del empleado
            var rEmpleado = await client.GetAsync($"api/empleados/{id}");
            if (rEmpleado.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Empleado = null;
                return;
            }

            if (!rEmpleado.IsSuccessStatusCode)
            {
                Error = $"La API respondió {(int)rEmpleado.StatusCode}";
                return;
            }

            var empleado = await rEmpleado.Content.ReadFromJsonAsync<EmpleadoBasicoViewModel>();

            // 2. Verificar si tiene asignaciones (7B)
            var rTiene = await client.GetAsync($"api/empleados/{id}/tiene-asignaciones");
            bool tieneAsignaciones = false;

            if (rTiene.IsSuccessStatusCode)
            {
                var resultado = await rTiene.Content.ReadFromJsonAsync<TieneAsignacionesDto>();
                tieneAsignaciones = resultado?.TieneAsignaciones ?? false;
            }

            Empleado = new EmpleadoDeleteViewModel
            {
                Id = empleado?.Id ?? 0,
                NombreCompleto = empleado?.NombreCompleto ?? string.Empty,
                Departamento = empleado?.Departamento ?? string.Empty,
                TieneAsignaciones = tieneAsignaciones
            };
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
        }
    }
}

public class EmpleadoBasicoViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
}

public class TieneAsignacionesDto
{
    public int EmpleadoId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public bool TieneAsignaciones { get; set; }
}

public class EmpleadoDeleteViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public bool TieneAsignaciones { get; set; }
}