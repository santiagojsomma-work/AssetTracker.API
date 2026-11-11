using AssetTracker.Web.Helpers;
using AssetTracker.Web.Services;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class DepreciacionModel : PageModel
{
    private readonly IAssetTrackerApiClient _api;

    public List<FilaDepreciacion> Filas { get; set; } = new();
    public List<ReporteEstadoViewModel> ReportePorEstado { get; set; } = new();
    public decimal TotalValorOriginal { get; set; }
    public decimal TotalAcumulada { get; set; }
    public decimal TotalValorEnLibros { get; set; }
    public string? Error { get; set; }

    public string EtiquetasJson => JsonSerializer.Serialize(Filas.Select(f => f.Codigo));
    public string ValoresLibrosJson => JsonSerializer.Serialize(Filas.Select(f => f.ValorEnLibros));
    public string AcumuladaJson => JsonSerializer.Serialize(Filas.Select(f => f.Acumulada));

    public DepreciacionModel(IAssetTrackerApiClient api) => _api = api;

    public async Task OnGetAsync()
    {
        var (reporte, _) = await _api.ObtenerReportePorEstadoAsync();
        ReportePorEstado = reporte ?? new();

        // CORRECCIÓN: la tupla tiene 3 elementos (Activos, TotalValor, Error)
        var (activos, _, error) = await _api.ObtenerActivosAsync();
        if (error != null) { Error = error; return; }

        Filas = (activos ?? new())
            .OrderBy(a => a.CodigoInterno)
            .Select(a =>
            {
                var d = DepreciacionHelper.Calcular(a);
                return new FilaDepreciacion
                {
                    Codigo = a.CodigoInterno ?? $"ACT-{a.Id:D4}",
                    Nombre = a.Nombre,
                    Tipo = a.TipoActivo,
                    FechaAdquisicion = a.FechaAdquisicion,
                    ValorOriginal = a.ValorOriginal,
                    AniosTranscurridos = d.AniosTranscurridos,
                    VidaUtilAnios = d.VidaUtilAnios,
                    DepreciacionAnual = d.DepreciacionAnual,
                    Acumulada = d.DepreciacionAcumulada,
                    ValorEnLibros = d.ValorEnLibros,
                    Porcentaje = d.PorcentajeDepreciado
                };
            })
            .ToList();

        TotalValorOriginal = Filas.Sum(f => f.ValorOriginal);
        TotalAcumulada = Filas.Sum(f => f.Acumulada);
        TotalValorEnLibros = Filas.Sum(f => f.ValorEnLibros);
    }

    public class FilaDepreciacion
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime FechaAdquisicion { get; set; }
        public decimal ValorOriginal { get; set; }
        public int AniosTranscurridos { get; set; }
        public int VidaUtilAnios { get; set; }
        public decimal DepreciacionAnual { get; set; }
        public decimal Acumulada { get; set; }
        public decimal ValorEnLibros { get; set; }
        public decimal Porcentaje { get; set; }
    }
}
