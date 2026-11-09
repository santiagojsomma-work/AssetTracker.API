using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public EstadisticasViewModel Estadisticas { get; set; } = new();
    public string? Error { get; set; }

    public IndexModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        var (stats, error) = await _api.ObtenerEstadisticasAsync();
        Estadisticas = stats ?? new();
        Error = error;
    }
}