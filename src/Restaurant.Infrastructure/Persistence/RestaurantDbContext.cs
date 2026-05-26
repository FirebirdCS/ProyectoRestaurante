using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Abstractions;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence;

/// <summary>
/// Contexto de persistencia (capa de acceso a datos).
/// Mapea el modelo relacional descrito en el diccionario de datos
/// sobre SQL Server LocalDB.
/// </summary>
public class RestaurantDbContext : DbContext, IAppDbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<CategoriaProducto> Categorias => Set<CategoriaProducto>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoDetalle> PedidoDetalles => Set<PedidoDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantDbContext).Assembly);
    }
}
