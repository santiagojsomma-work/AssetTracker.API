using AssetTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.API.Data;

public class AssetTrackerContext : DbContext
{
    public AssetTrackerContext(DbContextOptions<AssetTrackerContext> options) : base(options) { }

    public DbSet<Activo> Activos { get; set; }
    public DbSet<ActivoFisico> ActivosFisicos { get; set; }
    public DbSet<ActivoDigital> ActivosDigitales { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Asignacion> Asignaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de relaciones y restricciones (Consigna 2)
        modelBuilder.Entity<Asignacion>()
            .HasOne(a => a.Activo)
            .WithMany()
            .HasForeignKey(a => a.ActivoId)
            .OnDelete(DeleteBehavior.Restrict); // Evita borrar un activo si tiene asignaciones

        modelBuilder.Entity<Asignacion>()
            .HasOne(a => a.Empleado)
            .WithMany(e => e.Asignaciones)
            .HasForeignKey(a => a.EmpleadoId)
            .OnDelete(DeleteBehavior.Cascade); // Borra asignaciones si se borra el empleado
    }
}