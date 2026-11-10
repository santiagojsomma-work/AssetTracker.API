using AssetTracker.Web.Data;
using AssetTracker.Web.Helpers;
using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize(Roles = Permisos.RolesEmpleados)]
public class DeleteModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;
    private readonly UserManager<ApplicationUser> _userManager;

    public EmpleadoDeleteViewModel? Empleado { get; set; }
    public string? Error { get; set; }

    public DeleteModel(IAssetTrackerApiClient api, UserManager<ApplicationUser> userManager)
    {
        _api = api;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var bloqueo = await VerificarAlcanceDepartamentoAsync(id);
        if (bloqueo is not null) return bloqueo;

        await CargarEmpleadoAsync(id);
        return Empleado == null && Error == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var bloqueo = await VerificarAlcanceDepartamentoAsync(id);
        if (bloqueo is not null) return bloqueo;

        var resultado = await _api.EliminarEmpleadoAsync(id);
        if (resultado.Ok) return RedirectToPage("Index");

        Error = $"La API respondió {resultado.StatusCode}: {resultado.Error}";
        await CargarEmpleadoAsync(id);
        return Page();
    }

    /// <summary>
    /// R3: AdminDepartamento solo puede eliminar empleados de SU departamento.
    /// El Administrador tiene alcance global. Devuelve null si está permitido.
    /// </summary>
    private async Task<IActionResult?> VerificarAlcanceDepartamentoAsync(int id)
    {
        if (Permisos.EsSuperAdmin(User)) return null;

        var usuario = await _userManager.GetUserAsync(User);
        var (emp, _) = await _api.ObtenerEmpleadoAsync(id);
        if (emp == null) return NotFound();

        return string.Equals(usuario?.Departamento, emp.Departamento, StringComparison.OrdinalIgnoreCase)
            ? null
            : Forbid();
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