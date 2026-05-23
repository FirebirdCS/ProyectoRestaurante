using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class PedidoDetalleConfiguration : IEntityTypeConfiguration<PedidoDetalle>
{
    public void Configure(EntityTypeBuilder<PedidoDetalle> b)
    {
        b.ToTable("PedidoDetalle");
        b.HasCheckConstraint("CK_PedidoDetalle_cantidad", "[cantidad] > 0");
        b.HasCheckConstraint("CK_PedidoDetalle_precio", "[precio_unitario] >= 0");
        b.HasCheckConstraint("CK_PedidoDetalle_subtotal", "[subtotal] >= 0");
        b.HasKey(x => x.IdPedidoDetalle);
        b.Property(x => x.IdPedidoDetalle).HasColumnName("id_pedido_detalle").UseIdentityColumn();
        b.Property(x => x.IdPedido).HasColumnName("id_pedido").IsRequired();
        b.Property(x => x.IdProducto).HasColumnName("id_producto").IsRequired();
        b.Property(x => x.Cantidad).HasColumnName("cantidad").IsRequired();
        b.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario")
            .HasColumnType("decimal(10,2)").IsRequired();
        b.Property(x => x.Subtotal).HasColumnName("subtotal")
            .HasColumnType("decimal(10,2)").IsRequired();
        b.Property(x => x.Observaciones).HasColumnName("observaciones").HasMaxLength(200);

        b.HasIndex(x => x.IdPedido).HasDatabaseName("IX_PedidoDetalle_Pedido");
        b.HasIndex(x => x.IdProducto);

        b.HasOne(x => x.Pedido)
            .WithMany(p => p.Detalles)
            .HasForeignKey(x => x.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Producto)
            .WithMany(p => p.Detalles)
            .HasForeignKey(x => x.IdProducto)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
