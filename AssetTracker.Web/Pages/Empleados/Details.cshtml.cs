using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public EmpleadoDetalleViewModel? Empleado { get; set; }
    public string? Error { get; set; }

    public DetailsModel(IHttpClientFactory factory) => _factory = factory;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            var response = await client.GetAsync($"api/empleados/{id}/detalle");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                Error = $"La API respondió {(int)response.StatusCode}";
                return Page();
            }

            Empleado = await response.Content.ReadFromJsonAsync<EmpleadoDetalleViewModel>();
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
        }

        return Page();
    }
}

public class EmpleadoDetalleViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public int TotalAsignaciones { get; set; }
    public List<AsignacionDetalleViewModel> Asignaciones { get; set; } = new();
}

public class AsignacionDetalleViewModel
{
    public int Id { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public string ActivoNombre { get; set; } = string.Empty;
    public string ActivoEstado { get; set; } = string.Empty;
    public decimal ActivoValor { get; set; }
}