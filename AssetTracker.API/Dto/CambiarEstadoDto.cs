using System.ComponentModel.DataAnnotations;

namespace AssetTracker.API.Dto;

public class CambiarEstadoDto
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public string Estado { get; set; } = string.Empty;
}