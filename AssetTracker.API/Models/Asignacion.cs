namespace AssetTracker.API.Models;

public class Asignacion
{
    public int Id { get; set; }

    public int ActivoId { get; set; }
    public Activo Activo { get; set; } = null!;

    public int EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;

    public DateTime FechaAsignacion { get; set; } = DateTime.Now;
}