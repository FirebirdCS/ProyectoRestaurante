using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class MesaConfiguration : IEntityTypeConfiguration<Mesa>
{
    public void Configure(EntityTypeBuilder<Mesa> b)
    {
        b.ToTable("Mesa");
        b.HasCheckConstraint("CK_Mesa_numero", "[numero_mesa] > 0");
        b.HasCheckConstraint("CK_Mesa_capacidad", "[capacidad] BETWEEN 1 AND 20");
        b.HasCheckConstraint("CK_Mesa_estado",
            "[estado] IN ('DISPONIBLE','OCUPADA','EN_COBRO')");
        b.HasKey(x => x.IdMesa);
        b.Property(x => x.IdMesa).HasColumnName("id_mesa").UseIdentityColumn();
        b.Property(x => x.NumeroMesa).HasColumnName("numero_mesa").IsRequired();
        b.Property(x => x.Capacidad).HasColumnName("capacidad").IsRequired();
        b.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(20)
            .IsRequired().HasDefaultValue(EstadoMesa.Disponible);
        b.Property(x => x.Ubicacion).HasColumnName("ubicacion").HasMaxLength(50);
        b.Property(x => x.Activa).HasColumnName("activa").HasDefaultValue(true);

        b.HasIndex(x => x.NumeroMesa).IsUnique();
        b.HasIndex(x => x.Estado).HasDatabaseName("IX_Mesa_Estado");
    }
}
