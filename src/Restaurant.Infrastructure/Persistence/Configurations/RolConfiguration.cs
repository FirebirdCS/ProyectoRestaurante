using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> b)
    {
        b.ToTable("Rol");
        b.HasKey(x => x.IdRol);
        b.Property(x => x.IdRol).HasColumnName("id_rol").UseIdentityColumn();
        b.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(30).IsRequired();
        b.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(120);
        b.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);

        b.HasIndex(x => x.Nombre).IsUnique();
    }
}
