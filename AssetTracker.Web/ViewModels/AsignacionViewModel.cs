namespace AssetTracker.Web.ViewModels;

public class AsignacionViewModel
{
    public int Id { get; set; }
    public int ActivoId { get; set; }
    public int EmpleadoId { get; set; }
    public DateTime FechaAsignacion { get; set; }

    // Datos relacionados para mostrar en la vista
    public ActivoViewModel? Activo { get; set; }
    public EmpleadoViewModel? Empleado { get; set; }
}