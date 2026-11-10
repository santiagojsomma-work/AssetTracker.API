namespace AssetTracker.API.Models;

public class Empleado
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;

    // Campos nuevos (E1)
    public string? Documento { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Cargo { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(NombreCompleto))
            throw new ArgumentException("El nombre completo es obligatorio.");
        if (string.IsNullOrWhiteSpace(Departamento))
            throw new ArgumentException("El departamento es obligatorio.");
        if (!string.IsNullOrWhiteSpace(Correo) && !Correo.Contains("@"))
            throw new ArgumentException("El correo no es válido.");
        if (FechaIngreso.HasValue && FechaIngreso.Value > DateTime.Today)
            throw new ArgumentException("La fecha de ingreso no puede ser futura.");
    }
}