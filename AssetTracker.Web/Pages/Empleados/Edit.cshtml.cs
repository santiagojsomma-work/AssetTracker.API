using AssetTracker.Web.DTOs;
using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize]
public class EditModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    [BindProperty] public CrearEmpleadoDto Input { get; set; } = new();
    public string? Error { get; set; }

    public EditModel(IAssetTrackerApiClient api) => _api = api;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var (emp, error) = await _api.ObtenerEmpleadoAsync(id);
        if (error != null) { Error = error; return Page(); }
        if (emp == null) return NotFound();

        Input = new CrearEmpleadoDto
        {
            NombreCompleto = emp.NombreCompleto,
            Departamento = emp.Departamento,
            Documento = emp.Documento,
            Correo = emp.Correo,
            Telefono = emp.Telefono,
            Cargo = emp.Cargo,
            FechaIngreso = emp.FechaIngreso,
            Activo = emp.Activo
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid) return Page();

        var resultado = await _api.ActualizarEmpleadoAsync(id, Input);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        return Page();
    }
}