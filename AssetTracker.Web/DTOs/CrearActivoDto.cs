using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Web.DTOs;

public class CrearActivoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime FechaAdquisicion { get; set; }

    [Required(ErrorMessage = "El valor es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a cero")]
    public decimal ValorOriginal { get; set; }

    [Required(ErrorMessage = "El tipo es obligatorio")]
    public string TipoActivo { get; set; } = "Fisico";

    public string? NumeroSerie { get; set; }
    public string? Ubicacion { get; set; }
    public string? ClaveLicencia { get; set; }
    public DateTime? FechaVencimientoLicencia { get; set; }
}