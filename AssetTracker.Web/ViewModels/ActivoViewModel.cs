namespace AssetTracker.Web.ViewModels;

public class ActivoViewModel
{
    public int Id { get; set; }
    public string? CodigoInterno { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; }
    public decimal ValorOriginal { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Discriminator { get; set; } = string.Empty;

    // Propiedades específicas
    public string? NumeroSerie { get; set; }
    public string? Ubicacion { get; set; }
    public string? ClaveLicencia { get; set; }
    public DateTime? FechaVencimientoLicencia { get; set; }

    public string TipoActivo => Discriminator == "ActivoFisico" ? "Físico" : "Digital";
}