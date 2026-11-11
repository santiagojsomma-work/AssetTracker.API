using System.ComponentModel.DataAnnotations;

namespace AssetTracker.API.Dto;

public class CrearMantenimientoDto
{
    [Required] public int ActivoId { get; set; }

    [Required]
    [RegularExpression("^(Preventivo|Correctivo)$", ErrorMessage = "Debe ser 'Preventivo' o 'Correctivo'.")]
    public string Tipo { get; set; } = "Preventivo";

    [Required]
    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Proveedor { get; set; }

    [Required] public DateTime FechaProgramada { get; set; }
    public decimal Costo { get; set; }
}