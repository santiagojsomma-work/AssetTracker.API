using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Asignaciones;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public List<AsignacionViewModel> Asignaciones { get; set; } = new();
    public string? Error { get; set; }

    public IndexModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        var (asignaciones, error) = await _api.ObtenerAsignacionesAsync();
        Asignaciones = asignaciones ?? new();
        Error = error;
    }
}