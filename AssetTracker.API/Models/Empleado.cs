namespace AssetTracker.API.Models;

public class Empleado
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
}