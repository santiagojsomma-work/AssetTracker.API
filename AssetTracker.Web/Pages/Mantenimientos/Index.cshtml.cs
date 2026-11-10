using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetTracker.Web.Pages.Mantenimientos;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public List<MantenimientoViewModel> Mantenimientos { get; set; } = new();
    public string? Error { get; set; }

    [BindProperty(SupportsGet = true)] public string? FiltroEstado { get; set; }
    [BindProperty(SupportsGet = true)] public string? FiltroTipo { get; set; }

    // Estadísticas para la barra superior
    public int Total => Mantenimientos.Count;
    public int Pendientes => Mantenimientos.Count(m => m.EstadoReal is "Pendiente" or "En Progreso");
    public int Vencidos => Mantenimientos.Count(m => m.EstadoReal == "Vencido");
    public int Completados => Mantenimientos.Count(m => m.EstadoReal == "Completado");
    public decimal CostoTotal => Mantenimientos.Sum(m => m.Costo);

    public IndexModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        // Usamos 'listaMantenimientos' para evitar conflicto con la propiedad 'Mantenimientos'
        var (listaMantenimientos, error) = await _api.ObtenerMantenimientosAsync(FiltroEstado, FiltroTipo);
        Mantenimientos = listaMantenimientos ?? new();
        Error = error;
    }

    public async Task<IActionResult> OnPostCompletarAsync(int id)
    {
        var resultado = await _api.CambiarEstadoMantenimientoAsync(id, "Completado");
        if (!resultado.Ok) Error = $"Error al completar: {resultado.Error}";

        await RecargarAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCancelarAsync(int id)
    {
        var resultado = await _api.CambiarEstadoMantenimientoAsync(id, "Cancelado");
        if (!resultado.Ok) Error = $"Error al cancelar: {resultado.Error}";

        await RecargarAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        var resultado = await _api.EliminarMantenimientoAsync(id);
        if (!resultado.Ok) Error = $"Error al eliminar: {resultado.Error}";

        await RecargarAsync();
        return Page();
    }

    private async Task RecargarAsync()
    {
        var (listaMantenimientos, error) = await _api.ObtenerMantenimientosAsync(FiltroEstado, FiltroTipo);
        Mantenimientos = listaMantenimientos ?? new();
        if (Error == null) Error = error;
    }
}
