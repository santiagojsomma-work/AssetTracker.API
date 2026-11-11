namespace AssetTracker.API.Models;

public class Mantenimiento
{
    public int Id { get; set; }

    /// <summary>Código interno autogenerado (MNT-0001). Inmutable.</summary>
    public string? CodigoInterno { get; private set; }

    public int ActivoId { get; set; }
    public Activo Activo { get; set; } = null!;

    public string Tipo { get; set; } = "Preventivo"; // "Preventivo" o "Correctivo"
    public string Descripcion { get; set; } = string.Empty;
    public string? Proveedor { get; set; }

    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaRealizacion { get; set; }
    public decimal Costo { get; set; }

    public string Estado { get; private set; } = "Pendiente"; // Pendiente, En Progreso, Completado, Cancelado

    /// <summary>
    /// Evalúa si un mantenimiento pendiente está vencido.
    /// </summary>
    public string ObtenerEstadoReal()
    {
        if (Estado == "Completado" || Estado == "Cancelado") return Estado;
        if (Estado == "Pendiente" && FechaProgramada < DateTime.Today) return "Vencido";
        return Estado;
    }

    public void CambiarEstado(string nuevoEstado)
    {
        if (nuevoEstado is not ("Pendiente" or "En Progreso" or "Completado" or "Cancelado"))
            throw new ArgumentException("Estado no válido para mantenimiento.");

        Estado = nuevoEstado;
        if (nuevoEstado == "Completado" && FechaRealizacion == null)
            FechaRealizacion = DateTime.Today;
    }

    public void AsignarCodigoInterno(string codigo)
    {
        if (!string.IsNullOrWhiteSpace(CodigoInterno))
            throw new InvalidOperationException("El código interno ya fue asignado.");
        CodigoInterno = codigo;
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Descripcion))
            throw new ArgumentException("La descripción es obligatoria.");
        if (Costo < 0)
            throw new ArgumentException("El costo no puede ser negativo.");
        if (FechaProgramada == default)
            throw new ArgumentException("La fecha programada es obligatoria.");
    }
}