using AssetTracker.Web.DTOs;
using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class EditModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    [BindProperty] public CrearActivoDto Input { get; set; } = new();
    public string? Error { get; set; }

    public EditModel(IAssetTrackerApiClient api) => _api = api;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var (act, error) = await _api.ObtenerActivoAsync(id);
        if (error != null) { Error = error; return Page(); }
        if (act == null) return NotFound();

        Input = new CrearActivoDto
        {
            Nombre = act.Nombre,
            FechaAdquisicion = act.FechaAdquisicion,
            ValorOriginal = act.ValorOriginal,
            TipoActivo = act.TipoActivo == "Físico" ? "Fisico" : "Digital",
            NumeroSerie = act.NumeroSerie,
            Ubicacion = act.Ubicacion,
            ClaveLicencia = act.ClaveLicencia,
            FechaVencimientoLicencia = act.FechaVencimientoLicencia
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid) return Page();

        var resultado = await _api.ActualizarActivoAsync(id, Input);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        return Page();
    }
}