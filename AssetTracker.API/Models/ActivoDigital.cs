namespace AssetTracker.API.Models;

public class ActivoDigital : Activo
{
    public string ClaveLicencia { get; set; } = string.Empty;
    public DateTime FechaVencimientoLicencia { get; set; }
}