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
        modelBuilder.Entity<Asignacion>()
            .HasOne(a => a.Activo)
            .WithMany()
            .HasForeignKey(a => a.ActivoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asignacion>()
            .HasOne(a => a.Empleado)
            .WithMany(e => e.Asignaciones)
            .HasForeignKey(a => a.EmpleadoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}