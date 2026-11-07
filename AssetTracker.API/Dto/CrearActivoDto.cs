using System.ComponentModel.DataAnnotations;

namespace AssetTracker.API.Dto;

// DTO: Data Transfer Object
// Sirve para recibir datos del cliente sin exponer la entidad de dominio directamente.
// Esto evita el "overposting" (que el cliente envíe campos que no debería, como el Id).
public class CrearActivoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    // La validación de "no futura" se hace en el dominio, porque depende de DateTime.Now
    public DateTime FechaAdquisicion { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a cero.")]
    public decimal ValorOriginal { get; set; }

    // Campos específicos según el tipo de activo
    [Required(ErrorMessage = "El tipo de activo es obligatorio.")]
    public string TipoActivo { get; set; } = "Fisico"; // "Fisico" o "Digital"

    // Campos para ActivoFisico
    public string? NumeroSerie { get; set; }
    public string? Ubicacion { get; set; }

    // Campos para ActivoDigital
    public string? ClaveLicencia { get; set; }
    public DateTime? FechaVencimientoLicencia { get; set; }
}