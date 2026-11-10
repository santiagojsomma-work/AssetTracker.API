using AssetTracker.Web.DTOs;
using AssetTracker.Web.Helpers;
using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Mantenimientos;

[Authorize(Roles = Permisos.RolesInventario)]
public class CreateModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    [BindProperty] public CrearMantenimientoDto Input { get; set; } = new();
    public List<ActivoViewModel> Activos { get; set; } = new();
    public string? Error { get; set; }

    public CreateModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        Input.FechaProgramada = DateTime.Today.AddDays(7);
        var (activos, _, errActivos) = await _api.ObtenerActivosAsync();
        Activos = activos ?? new();
        Error = errActivos;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var (activosReload, _, errReload) = await _api.ObtenerActivosAsync();
            Activos = activosReload ?? new();
            Error = errReload;
            return Page();
        }

        var resultado = await _api.CrearMantenimientoAsync(Input);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        var (activosPost, _, errPost) = await _api.ObtenerActivosAsync();
        Activos = activosPost ?? new();
        Error = errPost ?? Error;
        return Page();
    }
}