namespace AssetTracker.API.Models;

public class ActivoFisico : Activo
{
    public string NumeroSerie { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;

    public override string ObtenerDetallesEspecificos()
    {
        return $"[FÍSICO] {Nombre} | Serie: {NumeroSerie} | Ubicación: {Ubicacion} | Valor: {ValorOriginal:C}";
    }
}