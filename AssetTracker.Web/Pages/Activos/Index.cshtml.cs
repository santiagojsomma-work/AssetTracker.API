using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public List<ActivoViewModel> Activos { get; set; } = new();
    public string? Error { get; set; }
    public decimal TotalValor { get; set; }
    public string[] Estados { get; set; } = Array.Empty<string>();

    [BindProperty(SupportsGet = true)] public string? FiltroEstado { get; set; }
    [BindProperty(SupportsGet = true)] public string? FiltroTexto { get; set; }
    [BindProperty(SupportsGet = true)] public string OrdenarPor { get; set; } = "nombre";
    [BindProperty(SupportsGet = true)] public bool Descendente { get; set; }

    public IndexModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        Estados = (await _api.ObtenerEstadosAsync()).Estados;

        if (!string.IsNullOrWhiteSpace(FiltroTexto))
        {
            var (activos, error) = await _api.BuscarActivosAsync(FiltroTexto);
            Activos = activos ?? new();
            TotalValor = Activos.Sum(a => a.ValorOriginal);
            Error = error;
        }
        else
        {
            var (activos, total, error) = await _api.ObtenerActivosAsync(FiltroEstado, OrdenarPor, Descendente);
            Activos = activos ?? new();
            TotalValor = total;
            Error = error;
        }
    }
}