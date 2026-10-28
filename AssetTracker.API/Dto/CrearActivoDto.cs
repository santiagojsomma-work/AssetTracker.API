namespace AssetTracker.API.Dto;

public class CrearActivoDto
{
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; }
    public decimal ValorOriginal { get; set; }
    public string TipoActivo { get; set; } = "Fisico";
    public string? NumeroSerie { get; set; }
    public string? Ubicacion { get; set; }
    public string? ClaveLicencia { get; set; }
    public DateTime? FechaVencimientoLicencia { get; set; }
}