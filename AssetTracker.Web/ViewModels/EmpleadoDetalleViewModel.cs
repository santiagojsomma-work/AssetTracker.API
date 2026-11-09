namespace AssetTracker.Web.ViewModels;

/// <summary>Contrato de GET /api/empleados/{id}/detalle</summary>
public class EmpleadoDetalleViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public int TotalAsignaciones { get; set; }
    public List<AsignacionDetalleViewModel> Asignaciones { get; set; } = new();
}

public class AsignacionDetalleViewModel
{
    public int Id { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public string ActivoNombre { get; set; } = string.Empty;
    public string ActivoEstado { get; set; } = string.Empty;
    public decimal ActivoValor { get; set; }
}