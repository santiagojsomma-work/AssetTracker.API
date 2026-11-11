namespace AssetTracker.API.Dto;

public class ActivoResumenDto
{
    public int Id { get; set; }
    public string? CodigoInterno { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; }
    public decimal ValorOriginal { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Discriminator { get; set; } = string.Empty;
}