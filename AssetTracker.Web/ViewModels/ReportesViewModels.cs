namespace AssetTracker.Web.ViewModels;

/// <summary>Contrato de GET /api/activos/estadisticas</summary>
public class EstadisticasViewModel
{
    public int TotalActivos { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorPromedio { get; set; }
    public decimal ValorMaximo { get; set; }
    public decimal ValorMinimo { get; set; }
    public int ActivosDisponibles { get; set; }
    public int ActivosAsignados { get; set; }
    public bool HayActivosDisponibles { get; set; }
    public bool HayActivosAsignados { get; set; }
    public int EstadosUnicos { get; set; }
}

/// <summary>Contrato de GET /api/activos/reporte (GroupBy por estado)</summary>
public class ReporteEstadoViewModel
{
    public string Estado { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorPromedio { get; set; }
    public decimal ValorMaximo { get; set; }
    public decimal ValorMinimo { get; set; }
}