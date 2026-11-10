using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Asignaciones;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public AsignacionViewModel? Asignacion { get; set; }
    public string? Error { get; set; }

    public DeleteModel(IAssetTrackerApiClient api) => _api = api;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        await CargarAsync(id);
        return Asignacion == null && Error == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var resultado = await _api.EliminarAsignacionAsync(id);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        await CargarAsync(id);
        return Page();
    }

    private async Task CargarAsync(int id)
    {
        var (asignacion, error) = await _api.ObtenerAsignacionAsync(id);
        Asignacion = asignacion;
        Error = error;
    }
}
