using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class CategoriaProductoConfiguration : IEntityTypeConfiguration<CategoriaProducto>
{
    public void Configure(EntityTypeBuilder<CategoriaProducto> b)
    {
        b.ToTable("CategoriaProducto");
        b.HasKey(x => x.IdCategoria);
        b.Property(x => x.IdCategoria).HasColumnName("id_categoria").UseIdentityColumn();
        b.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
        b.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(150);
        b.Property(x => x.Activa).HasColumnName("activa").HasDefaultValue(true);

        b.HasIndex(x => x.Nombre).IsUnique();
    }
}
