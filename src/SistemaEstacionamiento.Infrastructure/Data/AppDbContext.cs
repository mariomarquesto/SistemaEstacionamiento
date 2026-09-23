using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SistemaEstacionamiento.Core.Configuration;
using SistemaEstacionamiento.Core.Entities;

namespace SistemaEstacionamiento.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Lugar> Lugares => Set<Lugar>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<ConfiguracionEvento> Eventos => Set<ConfiguracionEvento>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lugar>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.Sector).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Fila).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Estado).HasMaxLength(15).IsRequired();
            entity.Property(e => e.Dni).HasMaxLength(10);              // ← NUEVO
            entity.Property(e => e.Patente).HasMaxLength(15);
            entity.Property(e => e.MetodoPago).HasMaxLength(20);
            entity.Property(e => e.Monto).HasPrecision(12, 2);
            entity.HasIndex(e => new { e.Sector, e.Estado });
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoVehiculo).HasMaxLength(30).IsRequired();
            entity.Property(e => e.LugarCodigo).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Dni).HasMaxLength(10);              // ← NUEVO
            entity.Property(e => e.Patente).HasMaxLength(15);
            entity.Property(e => e.MetodoPago).HasMaxLength(20);
            entity.Property(e => e.Operador).HasMaxLength(50);
            entity.Property(e => e.Monto).HasPrecision(12, 2);
        });

        modelBuilder.Entity<ConfiguracionEvento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();

            var comparer = new ValueComparer<List<SectorConfig>>(
                (a, b) => a!.SequenceEqual(b!),
                v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
                v => v.ToList()
            );

            entity.Property(e => e.Sectores)
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                      v => System.Text.Json.JsonSerializer.Deserialize<List<SectorConfig>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<SectorConfig>()
                  )
                  .Metadata.SetValueComparer(comparer);

            entity.Property(e => e.Sectores).HasColumnType("TEXT");
        });
    }
}