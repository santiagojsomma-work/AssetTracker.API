using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize(Roles = "Administrador")]
public class DeleteModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public EmpleadoDeleteViewModel? Empleado { get; set; }
    public string? Error { get; set; }

    public DeleteModel(IAssetTrackerApiClient api) => _api = api;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        await CargarEmpleadoAsync(id);
        return Empleado == null && Error == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var resultado = await _api.EliminarEmpleadoAsync(id);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        await CargarEmpleadoAsync(id);
        return Page();
    }

    private async Task CargarEmpleadoAsync(int id)
    {
        var (emp, error) = await _api.ObtenerEmpleadoAsync(id);
        if (emp == null)
        {
            Empleado = null;
            Error = error;
            return;
        }

        // 7B: avisar si la baja arrastra asignaciones en cascada
        var (tiene, _) = await _api.EmpleadoTieneAsignacionesAsync(id);
        Empleado = new EmpleadoDeleteViewModel
        {
            Id = emp.Id,
            NombreCompleto = emp.NombreCompleto,
            Departamento = emp.Departamento,
            TieneAsignaciones = tiene
        };
    }

    public class EmpleadoDeleteViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public bool TieneAsignaciones { get; set; }
    }
}
