namespace AssetTracker.API.Dto;

// DTO: Data Transfer Object
// Sirve para recibir datos del cliente sin exponer la entidad de dominio directamente.
// Esto evita el "overposting" (que el cliente envíe campos que no debería, como el Id).
public class CrearActivoDto
{
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; }
    public decimal ValorOriginal { get; set; }

    // Campos específicos según el tipo de activo
    public string TipoActivo { get; set; } = "Fisico"; // "Fisico" o "Digital"

    // Campos para ActivoFisico
    public string? NumeroSerie { get; set; }
    public string? Ubicacion { get; set; }

    // Campos para ActivoDigital
    public string? ClaveLicencia { get; set; }
    public DateTime? FechaVencimientoLicencia { get; set; }
}