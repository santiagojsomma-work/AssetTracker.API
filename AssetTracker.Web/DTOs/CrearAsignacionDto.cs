using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Web.DTOs;

public class CrearAsignacionDto
{
    [Required(ErrorMessage = "El ID del activo es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del activo debe ser mayor a cero")]
    public int ActivoId { get; set; }

    [Required(ErrorMessage = "El ID del empleado es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del empleado debe ser mayor a cero")]
    public int EmpleadoId { get; set; }
}