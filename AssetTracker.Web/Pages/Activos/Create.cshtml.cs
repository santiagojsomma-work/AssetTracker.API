using AssetTracker.Web.DTOs;
using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    [BindProperty] public CrearActivoDto Input { get; set; } = new();

    public CreateModel(IAssetTrackerApiClient api) => _api = api;

    public void OnGet()
    {
        Input.TipoActivo = "Fisico";
        Input.FechaAdquisicion = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var resultado = await _api.CrearActivoAsync(Input);
        if (resultado.Ok) return RedirectToPage("Index");

        ModelState.AddModelError("", $"La API respondió {resultado.StatusCode}: {resultado.Error}");
        return Page();
    }
}
