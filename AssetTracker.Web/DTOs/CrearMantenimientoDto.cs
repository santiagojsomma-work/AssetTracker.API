using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Web.DTOs;

public class CrearMantenimientoDto
{
    [Required(ErrorMessage = "El activo es obligatorio.")]
    public int ActivoId { get; set; }

    [Required]
    [RegularExpression("^(Preventivo|Correctivo)$", ErrorMessage = "Debe ser 'Preventivo' o 'Correctivo'.")]
    public string Tipo { get; set; } = "Preventivo";

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Proveedor { get; set; }

    [Required(ErrorMessage = "La fecha programada es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime FechaProgramada { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El costo no puede ser negativo.")]
    public decimal Costo { get; set; }
}