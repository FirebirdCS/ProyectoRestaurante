using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> b)
    {
        b.ToTable("Pedido");
        b.HasCheckConstraint("CK_Pedido_estado",
            "[estado] IN ('PENDIENTE','EN_PREPARACION','LISTO','CANCELADO')");
        b.HasKey(x => x.IdPedido);
        b.Property(x => x.IdPedido).HasColumnName("id_pedido").UseIdentityColumn();
        b.Property(x => x.IdCuenta).HasColumnName("id_cuenta").IsRequired();
        b.Property(x => x.IdUsuarioMesero).HasColumnName("id_usuario_mesero").IsRequired();
        b.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion")
            .HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        b.Property(x => x.FechaEnvioCocina).HasColumnName("fecha_envio_cocina").HasColumnType("datetime2");
        b.Property(x => x.FechaListo).HasColumnName("fecha_listo").HasColumnType("datetime2");
        b.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(20)
            .IsRequired().HasDefaultValue(EstadoPedido.Pendiente);
        b.Property(x => x.ObservacionGeneral).HasColumnName("observacion_general").HasMaxLength(200);

        b.Ignore(x => x.EnviadoACocina);
        b.HasIndex(x => x.IdUsuarioMesero);
        b.HasIndex(x => x.Estado).HasDatabaseName("IX_Pedido_Estado");
        b.HasIndex(x => new { x.IdCuenta, x.FechaCreacion }).HasDatabaseName("IX_Pedido_Cuenta_Fecha");
        b.HasIndex(x => x.FechaEnvioCocina).HasDatabaseName("IX_Pedido_FechaEnvioCocina");

        b.HasOne(x => x.Cuenta)
            .WithMany(c => c.Pedidos)
            .HasForeignKey(x => x.IdCuenta)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.UsuarioMesero)
            .WithMany(u => u.Pedidos)
            .HasForeignKey(x => x.IdUsuarioMesero)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
