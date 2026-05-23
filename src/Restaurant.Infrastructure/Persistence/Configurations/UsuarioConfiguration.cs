using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("Usuario");
        b.HasKey(x => x.IdUsuario);
        b.Property(x => x.IdUsuario).HasColumnName("id_usuario").UseIdentityColumn();
        b.Property(x => x.IdRol).HasColumnName("id_rol").IsRequired();
        b.Property(x => x.Nombres).HasColumnName("nombres").HasMaxLength(60).IsRequired();
        b.Property(x => x.Apellidos).HasColumnName("apellidos").HasMaxLength(60).IsRequired();
        b.Property(x => x.Username).HasColumnName("username").HasMaxLength(40).IsRequired();
        b.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        b.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        b.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion")
            .HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");

        b.Ignore(x => x.NombreCompleto);
        b.HasIndex(x => x.Username).IsUnique();
        b.HasIndex(x => x.IdRol);

        b.HasOne(x => x.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(x => x.IdRol)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
