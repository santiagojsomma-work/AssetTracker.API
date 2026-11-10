namespace AssetTracker.API.Dto;

public class MantenimientoResumenDto
{
    public int Id { get; set; }
    public string? CodigoInterno { get; set; }
    public int ActivoId { get; set; }
    public string NombreActivo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaRealizacion { get; set; }
    public decimal Costo { get; set; }
    public string EstadoReal { get; set; } = string.Empty; // Usa ObtenerEstadoReal()
}