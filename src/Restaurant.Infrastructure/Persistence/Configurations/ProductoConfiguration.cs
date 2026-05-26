using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> b)
    {
        b.ToTable("Producto");
        b.HasCheckConstraint("CK_Producto_precio", "[precio] >= 0");
        b.HasCheckConstraint("CK_Producto_tiempo", "[tiempo_preparacion_min] >= 0");
        b.HasKey(x => x.IdProducto);
        b.Property(x => x.IdProducto).HasColumnName("id_producto").UseIdentityColumn();
        b.Property(x => x.IdCategoria).HasColumnName("id_categoria").IsRequired();
        b.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(80).IsRequired();
        b.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(200);
        b.Property(x => x.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)").IsRequired();
        b.Property(x => x.Disponible).HasColumnName("disponible").HasDefaultValue(true);
        b.Property(x => x.TiempoPreparacionMin).HasColumnName("tiempo_preparacion_min");

        b.HasIndex(x => x.Nombre).IsUnique();
        b.HasIndex(x => x.IdCategoria);
        b.HasIndex(x => x.Disponible).HasDatabaseName("IX_Producto_Disponible");

        b.HasOne(x => x.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(x => x.IdCategoria)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
