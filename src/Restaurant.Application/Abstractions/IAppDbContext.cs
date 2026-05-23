using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Abstractions;

/// <summary>
/// Abstraccion de persistencia expuesta a la capa de aplicacion.
/// La implementacion concreta (RestaurantDbContext) vive en Infrastructure,
/// manteniendo la direccion de dependencias de la arquitectura en capas.
/// </summary>
public interface IAppDbContext
{
    DbSet<Rol> Roles { get; }
    DbSet<Usuario> Usuarios { get; }
    DbSet<Mesa> Mesas { get; }
    DbSet<CategoriaProducto> Categorias { get; }
    DbSet<Producto> Productos { get; }
    DbSet<Cuenta> Cuentas { get; }
    DbSet<Pedido> Pedidos { get; }
    DbSet<PedidoDetalle> PedidoDetalles { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
