using AssetTracker.Web.Helpers;
using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Activos;

[Authorize(Roles = Permisos.RolesSuperAdmin)]
public class DeleteModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public ActivoViewModel? Activo { get; set; }
    public string? Error { get; set; }

    public DeleteModel(IAssetTrackerApiClient api) => _api = api;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var (act, error) = await _api.ObtenerActivoAsync(id);
        if (error != null) { Error = error; return Page(); }
        Activo = act;
        return Activo == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var resultado = await _api.EliminarActivoAsync(id);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        var (act, _) = await _api.ObtenerActivoAsync(id);
        Activo = act;
        return Page();
    }
}