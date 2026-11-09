using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public EmpleadoDetalleViewModel? Empleado { get; set; }
    public string? Error { get; set; }

    public DetailsModel(IAssetTrackerApiClient api) => _api = api;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var (detalle, error) = await _api.ObtenerEmpleadoDetalleAsync(id);
        Empleado = detalle;
        Error = error;
        if (Empleado == null && Error == null) return NotFound();
        return Page();
    }
}