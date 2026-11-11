using System.ComponentModel.DataAnnotations;

namespace AssetTracker.API.Dto;

public class CrearEmpleadoDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El departamento es obligatorio.")]
    [StringLength(50, ErrorMessage = "El departamento no puede superar los 50 caracteres.")]
    public string Departamento { get; set; } = string.Empty;

    // ✅ NUEVAS PROPIEDADES (faltan en tu archivo actual)
    [StringLength(20)]
    public string? Documento { get; set; }

    [StringLength(100)]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    public string? Correo { get; set; }

    [StringLength(20)]
    [Phone(ErrorMessage = "El teléfono no es válido.")]
    public string? Telefono { get; set; }

    [StringLength(50)]
    public string? Cargo { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaIngreso { get; set; }

    public bool Activo { get; set; } = true;
}