namespace AssetTracker.API.Dto;

/// <summary>
/// DTO para mostrar un empleado con sus asignaciones completas
/// </summary>
public class EmpleadoDetalleDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public int TotalAsignaciones { get; set; }
    public List<AsignacionDetalleDto> Asignaciones { get; set; } = new();
}

public class AsignacionDetalleDto
{
    public int Id { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public string ActivoNombre { get; set; } = string.Empty;
    public string ActivoEstado { get; set; } = string.Empty;
    public decimal ActivoValor { get; set; }
}