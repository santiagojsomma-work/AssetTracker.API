namespace AssetTracker.Web.ViewModels;

public class EmpleadoViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Cargo { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public bool Activo { get; set; } = true;
}