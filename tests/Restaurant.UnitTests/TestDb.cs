using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.UnitTests;

/// <summary>
/// Fabrica de RestaurantDbContext sobre el proveedor EF Core InMemory
/// para probar las reglas de negocio sin SQL Server.
/// </summary>
internal static class TestDb
{
    public static RestaurantDbContext Create()
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseInMemoryDatabase($"test-{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;
        return new RestaurantDbContext(options);
    }

    /// <summary>Crea un escenario base: rol, mesero, mesa disponible y un producto.</summary>
    public static (Mesa mesa, Usuario mesero, Producto producto) SeedBase(RestaurantDbContext db)
    {
        var rol = new Rol { Nombre = RolNombre.Mesero, Activo = true };
        var mesero = new Usuario
        {
            Nombres = "Test", Apellidos = "Mesero", Username = "t",
            PasswordHash = "x", Activo = true, Rol = rol, FechaCreacion = DateTime.Now
        };
        var cat = new CategoriaProducto { Nombre = "Cat", Activa = true };
        var prod = new Producto { Nombre = "Cafe", Precio = 20m, Disponible = true, Categoria = cat };
        var mesa = new Mesa
        {
            NumeroMesa = 1, Capacidad = 4,
            Estado = EstadoMesa.Disponible, Activa = true
        };

        db.AddRange(rol, mesero, cat, prod, mesa);
        db.SaveChanges();
        return (mesa, mesero, prod);
    }
}
