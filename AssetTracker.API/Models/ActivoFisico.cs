namespace AssetTracker.API.Models;

public class ActivoFisico : Activo
{
    public string NumeroSerie { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
}