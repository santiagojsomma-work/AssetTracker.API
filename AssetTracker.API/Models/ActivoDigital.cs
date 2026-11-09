namespace AssetTracker.API.Models;

public class ActivoDigital : Activo
{
    public string ClaveLicencia { get; set; } = string.Empty;

    /// <summary>
    /// Vencimiento de licencia nullable: null = sin vencimiento (15A).
    /// Reemplaza al centinela DateTime.MaxValue (Primitive Obsession).
    /// </summary>
    public DateTime? FechaVencimientoLicencia { get; set; }
}