using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Abstractions;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Infrastructure.Persistence;

/// <summary>
/// Aplica migraciones pendientes y carga datos semilla (roles, usuarios,
/// mesas, categorias y productos) para que el sistema sea operable sin
/// pantallas de administracion en esta fase minima.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(RestaurantDbContext db, IPasswordHasher hasher)
    {
        await db.Database.MigrateAsync();
        await SeedRolesAsync(db);
        await SeedUsuariosAsync(db, hasher);
        await SeedMesasAsync(db);
        await SeedMenuAsync(db);
    }

    private static async Task SeedRolesAsync(RestaurantDbContext db)
    {
        if (await db.Roles.AnyAsync()) return;

        db.Roles.AddRange(
            new Rol { Nombre = RolNombre.Administrador, Descripcion = "Acceso total y administracion", Activo = true },
            new Rol { Nombre = RolNombre.Mesero, Descripcion = "Atiende mesas y registra pedidos", Activo = true },
            new Rol { Nombre = RolNombre.Cocina, Descripcion = "Prepara y actualiza pedidos", Activo = true },
            new Rol { Nombre = RolNombre.Cajero, Descripcion = "Genera y cierra cuentas", Activo = true });
        await db.SaveChangesAsync();
    }

    private static async Task SeedUsuariosAsync(RestaurantDbContext db, IPasswordHasher hasher)
    {
        if (await db.Usuarios.AnyAsync()) return;

        var roles = await db.Roles.ToDictionaryAsync(r => r.Nombre, r => r.IdRol);

        db.Usuarios.AddRange(
            NuevoUsuario("Abel", "Sanchez", "admin", "Admin123!", roles[RolNombre.Administrador], hasher),
            NuevoUsuario("Maria", "Lopez", "mesero1", "Mesero123!", roles[RolNombre.Mesero], hasher),
            NuevoUsuario("Carlos", "Ramirez", "cocina1", "Cocina123!", roles[RolNombre.Cocina], hasher),
            NuevoUsuario("Lucia", "Gomez", "cajero1", "Cajero123!", roles[RolNombre.Cajero], hasher));
        await db.SaveChangesAsync();
    }

    private static Usuario NuevoUsuario(string nombres, string apellidos, string username,
        string password, int idRol, IPasswordHasher hasher) => new()
    {
        Nombres = nombres,
        Apellidos = apellidos,
        Username = username,
        PasswordHash = hasher.Hash(password),
        IdRol = idRol,
        Activo = true,
        FechaCreacion = DateTime.Now
    };

    private static async Task SeedMesasAsync(RestaurantDbContext db)
    {
        if (await db.Mesas.AnyAsync()) return;

        // El restaurante cuenta con 18 mesas (descripcion del problema).
        for (var i = 1; i <= 18; i++)
        {
            db.Mesas.Add(new Mesa
            {
                NumeroMesa = i,
                Capacidad = (byte)(i % 3 == 0 ? 6 : 4),
                Estado = EstadoMesa.Disponible,
                Ubicacion = i <= 12 ? "Salon principal" : "Terraza",
                Activa = true
            });
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedMenuAsync(RestaurantDbContext db)
    {
        if (await db.Categorias.AnyAsync()) return;

        var entradas = new CategoriaProducto { Nombre = "Entradas", Descripcion = "Para iniciar", Activa = true };
        var fuertes = new CategoriaProducto { Nombre = "Platos fuertes", Descripcion = "Platillos principales", Activa = true };
        var bebidas = new CategoriaProducto { Nombre = "Bebidas", Descripcion = "Frias y calientes", Activa = true };
        var postres = new CategoriaProducto { Nombre = "Postres", Descripcion = "Para finalizar", Activa = true };
        db.Categorias.AddRange(entradas, fuertes, bebidas, postres);
        await db.SaveChangesAsync();

        db.Productos.AddRange(
            new Producto { Categoria = entradas, Nombre = "Sopa de tortilla", Precio = 35m, Disponible = true, TiempoPreparacionMin = 10 },
            new Producto { Categoria = entradas, Nombre = "Guacamole con totopos", Precio = 45m, Disponible = true, TiempoPreparacionMin = 8 },
            new Producto { Categoria = entradas, Nombre = "Tostadas de pollo", Precio = 40m, Disponible = true, TiempoPreparacionMin = 10 },
            new Producto { Categoria = fuertes, Nombre = "Carne asada", Precio = 95m, Disponible = true, TiempoPreparacionMin = 20 },
            new Producto { Categoria = fuertes, Nombre = "Pollo a la plancha", Precio = 80m, Disponible = true, TiempoPreparacionMin = 18 },
            new Producto { Categoria = fuertes, Nombre = "Enchiladas suizas", Precio = 75m, Disponible = true, TiempoPreparacionMin = 15 },
            new Producto { Categoria = fuertes, Nombre = "Pescado al mojo de ajo", Precio = 110m, Disponible = true, TiempoPreparacionMin = 22 },
            new Producto { Categoria = bebidas, Nombre = "Agua de horchata", Precio = 20m, Disponible = true, TiempoPreparacionMin = 3 },
            new Producto { Categoria = bebidas, Nombre = "Refresco", Precio = 18m, Disponible = true, TiempoPreparacionMin = 1 },
            new Producto { Categoria = bebidas, Nombre = "Cafe americano", Precio = 22m, Disponible = true, TiempoPreparacionMin = 4 },
            new Producto { Categoria = bebidas, Nombre = "Limonada", Precio = 20m, Disponible = true, TiempoPreparacionMin = 3 },
            new Producto { Categoria = postres, Nombre = "Flan napolitano", Precio = 38m, Disponible = true, TiempoPreparacionMin = 5 },
            new Producto { Categoria = postres, Nombre = "Pastel de chocolate", Precio = 42m, Disponible = true, TiempoPreparacionMin = 5 },
            new Producto { Categoria = postres, Nombre = "Helado de vainilla", Precio = 30m, Disponible = true, TiempoPreparacionMin = 2 });
        await db.SaveChangesAsync();
    }
}
