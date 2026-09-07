namespace AssetTracker.API.Models;

public class Empleado
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;

    // Relación 1 a muchos (Consigna 2)
    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
}