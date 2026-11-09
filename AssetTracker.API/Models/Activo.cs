namespace AssetTracker.API.Models;

/// <summary>
/// Entidad base abstracta de la jerarquía TPH de activos.
/// </summary>
public abstract class Activo
{
    public int Id { get; set; }

    /// <summary>Código interno autogenerado (ACT-0001). Inmutable.</summary>
    public string? CodigoInterno { get; private set; }

    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; }
    public decimal ValorOriginal { get; set; }

    public string Estado { get; private set; } = EstadosActivo.Disponible;

    /// <summary>Cambia el estado validando contra el catálogo de 10 estados.</summary>
    public void CambiarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
            throw new ArgumentException("El estado no puede estar vacío.");

        if (!EstadosActivo.EsValido(nuevoEstado))
            throw new ArgumentException(
                $"Estado no válido: '{nuevoEstado}'. Estados permitidos: {string.Join(", ", EstadosActivo.Todos)}.");

        Estado = nuevoEstado;
    }

    /// <summary>Asigna el código interno una única vez (inmutabilidad).</summary>
    public void AsignarCodigoInterno(string codigo)
    {
        if (!string.IsNullOrWhiteSpace(CodigoInterno))
            throw new InvalidOperationException("El código interno ya fue asignado y es inmutable.");

        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código interno no puede estar vacío.");

        CodigoInterno = codigo;
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre del activo es obligatorio.");
        if (ValorOriginal <= 0)
            throw new ArgumentException("El valor del activo debe ser mayor a cero.");
        if (FechaAdquisicion > DateTime.Now)
            throw new ArgumentException("La fecha de adquisición no puede ser futura.");
    }
}