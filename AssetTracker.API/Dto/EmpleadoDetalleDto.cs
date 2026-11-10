namespace AssetTracker.API.Dto;

public class EmpleadoDetalleDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Cargo { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public bool Activo { get; set; }
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