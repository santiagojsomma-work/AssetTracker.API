using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Helpers;

/// <summary>
/// Depreciación lineal (método de línea recta):
/// depreciación anual = ValorOriginal / vida útil en años.
/// Vida útil: físicos 5 años, digitales 3 años.
/// </summary>
public static class DepreciacionHelper
{
    public static int VidaUtilAnios(string tipoActivo) => tipoActivo == "Físico" ? 5 : 3;

    public static DetalleDepreciacion Calcular(ActivoViewModel activo, DateTime? fechaCorte = null)
    {
        var corte = (fechaCorte ?? DateTime.Today).Date;
        var vidaUtil = VidaUtilAnios(activo.TipoActivo);

        // Años cumplidos desde la adquisición
        int anios = corte.Year - activo.FechaAdquisicion.Year;
        if (corte < activo.FechaAdquisicion.AddYears(anios)) anios--;
        anios = Math.Max(0, anios);

        var depreciacionAnual = Math.Round(activo.ValorOriginal / vidaUtil, 2);
        var aniosDepreciables = Math.Min(anios, vidaUtil); // nunca depreciar por debajo de 0
        var acumulada = Math.Round(depreciacionAnual * aniosDepreciables, 2);
        var valorEnLibros = Math.Max(0, activo.ValorOriginal - acumulada);
        var porcentaje = activo.ValorOriginal > 0
            ? Math.Round(acumulada / activo.ValorOriginal * 100, 1)
            : 0;

        return new DetalleDepreciacion
        {
            AniosTranscurridos = anios,
            VidaUtilAnios = vidaUtil,
            DepreciacionAnual = depreciacionAnual,
            DepreciacionAcumulada = acumulada,
            ValorEnLibros = valorEnLibros,
            PorcentajeDepreciado = porcentaje
        };
    }
}

public class DetalleDepreciacion
{
    public int AniosTranscurridos { get; set; }
    public int VidaUtilAnios { get; set; }
    public decimal DepreciacionAnual { get; set; }
    public decimal DepreciacionAcumulada { get; set; }
    public decimal ValorEnLibros { get; set; }
    public decimal PorcentajeDepreciado { get; set; }
}