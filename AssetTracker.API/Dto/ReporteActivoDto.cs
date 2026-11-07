namespace AssetTracker.API.Dto;

/// <summary>
/// DTO para reportes agrupados por estado
/// </summary>
public class ReporteActivoDto
{
    public string Estado { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorPromedio { get; set; }
    public decimal ValorMaximo { get; set; }
    public decimal ValorMinimo { get; set; }
}