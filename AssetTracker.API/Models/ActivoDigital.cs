namespace AssetTracker.API.Models;

public class ActivoDigital : Activo
{
    public string ClaveLicencia { get; set; } = string.Empty;
    public DateTime FechaVencimientoLicencia { get; set; }

    public override string ObtenerDetallesEspecificos()
    {
        return $"[DIGITAL] {Nombre} | Licencia: {ClaveLicencia} | Vence: {FechaVencimientoLicencia:dd/MM/yyyy} | Valor: {ValorOriginal:C}";
    }
}