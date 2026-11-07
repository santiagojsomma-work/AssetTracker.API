using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Web.DTOs;

public class CrearEmpleadoDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El departamento es obligatorio")]
    [StringLength(50, ErrorMessage = "El departamento no puede superar los 50 caracteres")]
    public string Departamento { get; set; } = string.Empty;
}