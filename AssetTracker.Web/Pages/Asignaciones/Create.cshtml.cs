using AssetTracker.Web.DTOs;
using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Asignaciones;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    [BindProperty] public CrearAsignacionDto Input { get; set; } = new();
    public List<ActivoViewModel> ActivosDisponibles { get; set; } = new();
    public List<EmpleadoViewModel> Empleados { get; set; } = new();
    public string? Error { get; set; }

    public CreateModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        await CargarListasAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return Page();
        }

        var resultado = await _api.CrearAsignacionAsync(Input);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        await CargarListasAsync();
        return Page();
    }

    private async Task CargarListasAsync()
    {
        // CORRECCIÓN: la tupla tiene 3 elementos (Activos, TotalValor, Error)
        var (activos, _, errActivos) = await _api.ObtenerActivosAsync(estado: "Disponible");
        var (empleados, errEmpleados) = await _api.ObtenerEmpleadosAsync();

        ActivosDisponibles = activos ?? new();
        Empleados = empleados ?? new();
        if (Error == null) Error = errActivos ?? errEmpleados;
    }
}