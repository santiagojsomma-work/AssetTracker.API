using AssetTracker.Web.Helpers;
using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public ActivoViewModel? Activo { get; set; }
    public string[] Estados { get; set; } = Array.Empty<string>();
    public string? Error { get; set; }

    [BindProperty] public string NuevoEstado { get; set; } = string.Empty;

    public DetailsModel(IAssetTrackerApiClient api) => _api = api;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        await CargarAsync(id);
        return Activo == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostCambiarEstadoAsync(int id)
    {
        // R3: solo Inventario/Administrador puede cambiar estados
        if (!Permisos.GestionaInventario(User)) return Forbid();

        var resultado = await _api.CambiarEstadoActivoAsync(id, NuevoEstado);
        if (resultado.Ok) return RedirectToPage("Details", new { id });

        Error = $"No se pudo cambiar el estado: {resultado.Error}";
        await CargarAsync(id);
        return Page();
    }

    private async Task CargarAsync(int id)
    {
        Estados = (await _api.ObtenerEstadosAsync()).Estados;
        var (act, error) = await _api.ObtenerActivoAsync(id);
        Activo = act;
        Error = error;
    }
}