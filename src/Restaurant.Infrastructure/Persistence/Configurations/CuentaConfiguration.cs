using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class CuentaConfiguration : IEntityTypeConfiguration<Cuenta>
{
    public void Configure(EntityTypeBuilder<Cuenta> b)
    {
        b.ToTable("Cuenta");
        b.HasCheckConstraint("CK_Cuenta_estado",
            "[estado] IN ('ABIERTA','CERRADA','ANULADA')");
        b.HasCheckConstraint("CK_Cuenta_subtotal", "[subtotal] >= 0");
        b.HasCheckConstraint("CK_Cuenta_impuesto", "[impuesto] >= 0");
        b.HasCheckConstraint("CK_Cuenta_total", "[total] >= 0");
        b.HasKey(x => x.IdCuenta);
        b.Property(x => x.IdCuenta).HasColumnName("id_cuenta").UseIdentityColumn();
        b.Property(x => x.IdMesa).HasColumnName("id_mesa").IsRequired();
        b.Property(x => x.IdUsuarioApertura).HasColumnName("id_usuario_apertura").IsRequired();
        b.Property(x => x.IdUsuarioCierre).HasColumnName("id_usuario_cierre");
        b.Property(x => x.FechaApertura).HasColumnName("fecha_apertura")
            .HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        b.Property(x => x.FechaCierre).HasColumnName("fecha_cierre").HasColumnType("datetime2");
        b.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(15)
            .IsRequired().HasDefaultValue(EstadoCuenta.Abierta);
        b.Property(x => x.Subtotal).HasColumnName("subtotal")
            .HasColumnType("decimal(10,2)").HasDefaultValue(0m);
        b.Property(x => x.Impuesto).HasColumnName("impuesto")
            .HasColumnType("decimal(10,2)").HasDefaultValue(0m);
        b.Property(x => x.Total).HasColumnName("total")
            .HasColumnType("decimal(10,2)").HasDefaultValue(0m);
        b.Property(x => x.Observaciones).HasColumnName("observaciones").HasMaxLength(200);

        b.HasIndex(x => x.IdMesa);
        b.HasIndex(x => x.IdUsuarioApertura);
        b.HasIndex(x => x.IdUsuarioCierre);
        b.HasIndex(x => x.Estado).HasDatabaseName("IX_Cuenta_Estado");
        b.HasIndex(x => x.FechaCierre).HasDatabaseName("IX_Cuenta_FechaCierre");

        b.HasOne(x => x.Mesa)
            .WithMany(m => m.Cuentas)
            .HasForeignKey(x => x.IdMesa)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.UsuarioApertura)
            .WithMany(u => u.CuentasAbiertas)
            .HasForeignKey(x => x.IdUsuarioApertura)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.UsuarioCierre)
            .WithMany()
            .HasForeignKey(x => x.IdUsuarioCierre)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
