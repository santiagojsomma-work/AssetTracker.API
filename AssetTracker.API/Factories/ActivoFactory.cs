using AssetTracker.API.Dto;
using AssetTracker.API.Models;

namespace AssetTracker.API.Factories;

/// <summary>
/// Centraliza la creación y actualización de activos según su tipo.
/// Elimina las Switch/If-Else Chains del controller (code smell S4):
/// el controller ya no sabe cómo se construye cada activo.
/// </summary>
public static class ActivoFactory
{
    /// <summary>Crea el activo según el tipo del DTO.</summary>
    /// <exception cref="ArgumentException">Si el tipo no es Fisico o Digital.</exception>
    public static Activo Crear(CrearActivoDto dto)
    {
        return dto.TipoActivo?.ToLower() switch
        {
            "fisico" => new ActivoFisico
            {
                Nombre = dto.Nombre,
                FechaAdquisicion = dto.FechaAdquisicion,
                ValorOriginal = dto.ValorOriginal,
                NumeroSerie = dto.NumeroSerie ?? string.Empty,
                Ubicacion = dto.Ubicacion ?? string.Empty
            },
            "digital" => new ActivoDigital
            {
                Nombre = dto.Nombre,
                FechaAdquisicion = dto.FechaAdquisicion,
                ValorOriginal = dto.ValorOriginal,
                ClaveLicencia = dto.ClaveLicencia ?? string.Empty,
                FechaVencimientoLicencia = dto.FechaVencimientoLicencia
            },
            _ => throw new ArgumentException("Tipo inválido. Use 'Fisico' o 'Digital'.")
        };
    }

    /// <summary>Aplica los cambios del DTO sobre un activo existente.</summary>
    public static void AplicarCambios(Activo activo, CrearActivoDto dto)
    {
        activo.Nombre = dto.Nombre;
        activo.FechaAdquisicion = dto.FechaAdquisicion;
        activo.ValorOriginal = dto.ValorOriginal;

        switch (activo)
        {
            case ActivoFisico fisico:
                fisico.NumeroSerie = dto.NumeroSerie ?? string.Empty;
                fisico.Ubicacion = dto.Ubicacion ?? string.Empty;
                break;
            case ActivoDigital digital:
                digital.ClaveLicencia = dto.ClaveLicencia ?? string.Empty;
                digital.FechaVencimientoLicencia = dto.FechaVencimientoLicencia;
                break;
        }
    }

    /// <summary>Tipo real del activo, para validaciones de coherencia.</summary>
    public static string TipoDe(Activo activo) => activo is ActivoFisico ? "fisico" : "digital";
}