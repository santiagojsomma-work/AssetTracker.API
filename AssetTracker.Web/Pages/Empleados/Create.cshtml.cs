using AssetTracker.Web.DTOs;
using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    [BindProperty] public CrearEmpleadoDto Input { get; set; } = new();
    public string? Error { get; set; }

    public CreateModel(IAssetTrackerApiClient api) => _api = api;

    public void OnGet()
    {
        Input.Activo = true;
        Input.FechaIngreso = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var resultado = await _api.CrearEmpleadoAsync(Input);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        return Page();
    }
}
