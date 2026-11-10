using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Empleados;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public List<EmpleadoViewModel> Empleados { get; set; } = new();
    public string? Error { get; set; }

    [BindProperty(SupportsGet = true)] public string? FiltroDepartamento { get; set; }
    [BindProperty(SupportsGet = true)] public string? FiltroEstado { get; set; }

    public int Total => Empleados.Count;
    public int Activos => Empleados.Count(e => e.Activo);
    public int Inactivos => Empleados.Count(e => !e.Activo);
    public List<string> Departamentos => Empleados
        .Select(e => e.Departamento)
        .Where(d => !string.IsNullOrWhiteSpace(d))
        .Distinct()
        .OrderBy(d => d)
        .ToList();

    public IndexModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        var (empleados, error) = await _api.ObtenerEmpleadosAsync();
        var lista = empleados ?? new();

        if (!string.IsNullOrWhiteSpace(FiltroDepartamento))
            lista = lista.Where(e => e.Departamento == FiltroDepartamento).ToList();
        if (!string.IsNullOrWhiteSpace(FiltroEstado))
        {
            bool estado = FiltroEstado == "activo";
            lista = lista.Where(e => e.Activo == estado).ToList();
        }

        Empleados = lista;
        Error = error;
    }
}