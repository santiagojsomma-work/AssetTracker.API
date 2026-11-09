using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public List<EmpleadoViewModel> Empleados { get; set; } = new();
    public string? Error { get; set; }

    public IndexModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        var (empleados, error) = await _api.ObtenerEmpleadosAsync();
        Empleados = empleados ?? new();
        Error = error;
    }
}