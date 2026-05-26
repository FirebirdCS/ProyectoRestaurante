using Restaurant.Application.Services;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Xunit;

namespace Restaurant.UnitTests;

public class BillingTests
{
    private static Cuenta NuevaCuenta(Restaurant.Infrastructure.Persistence.RestaurantDbContext db,
        Mesa mesa, Usuario mesero)
    {
        var cuenta = new Cuenta
        {
            Mesa = mesa,
            UsuarioApertura = mesero,
            FechaApertura = DateTime.Now,
            Estado = EstadoCuenta.Abierta
        };
        db.Cuentas.Add(cuenta);
        db.SaveChanges();
        return cuenta;
    }

    private static void AgregarPedido(Restaurant.Infrastructure.Persistence.RestaurantDbContext db,
        Cuenta cuenta, Usuario mesero, Producto prod, short cant, string estado)
    {
        var pedido = new Pedido
        {
            IdCuenta = cuenta.IdCuenta,
            UsuarioMesero = mesero,
            FechaCreacion = DateTime.Now,
            Estado = estado
        };
        pedido.Detalles.Add(new PedidoDetalle
        {
            IdProducto = prod.IdProducto,
            Cantidad = cant,
            PrecioUnitario = prod.Precio,
            Subtotal = prod.Precio * cant
        });
        db.Pedidos.Add(pedido);
        db.SaveChanges();
    }

    [Fact]
    public async Task Consumo_excluye_cancelados_y_aplica_impuesto_12()
    {
        using var db = TestDb.Create();
        var (mesa, mesero, prod) = TestDb.SeedBase(db);
        var cuenta = NuevaCuenta(db, mesa, mesero);
        AgregarPedido(db, cuenta, mesero, prod, 3, EstadoPedido.Listo);      // 3 x 20 = 60
        AgregarPedido(db, cuenta, mesero, prod, 5, EstadoPedido.Cancelado);  // excluido

        var sut = new CuentaService(db);
        var consumo = await sut.ObtenerConsumoAsync(cuenta.IdCuenta);

        Assert.NotNull(consumo);
        Assert.Single(consumo!.Lineas);
        Assert.Equal(3, consumo.Lineas[0].Cantidad);
        Assert.Equal(60m, consumo.Subtotal);
        Assert.Equal(7.20m, consumo.Impuesto);
        Assert.Equal(67.20m, consumo.Total);
    }

    [Fact]
    public async Task Cerrar_cuenta_sin_consumo_falla()
    {
        using var db = TestDb.Create();
        var (mesa, mesero, _) = TestDb.SeedBase(db);
        var cuenta = NuevaCuenta(db, mesa, mesero);

        var sut = new CuentaService(db);
        var r = await sut.CerrarCuentaAsync(cuenta.IdCuenta, mesero.IdUsuario);

        Assert.False(r.Ok);
    }

    [Fact]
    public async Task Cerrar_cuenta_con_consumo_libera_mesa_y_persiste_total()
    {
        using var db = TestDb.Create();
        var (mesa, mesero, prod) = TestDb.SeedBase(db);
        mesa.Estado = EstadoMesa.Ocupada;
        var cuenta = NuevaCuenta(db, mesa, mesero);
        AgregarPedido(db, cuenta, mesero, prod, 2, EstadoPedido.Listo); // 40

        var sut = new CuentaService(db);
        var r = await sut.CerrarCuentaAsync(cuenta.IdCuenta, mesero.IdUsuario);

        Assert.True(r.Ok);
        var cerrada = db.Cuentas.Find(cuenta.IdCuenta)!;
        Assert.Equal(EstadoCuenta.Cerrada, cerrada.Estado);
        Assert.NotNull(cerrada.FechaCierre);
        Assert.Equal(mesero.IdUsuario, cerrada.IdUsuarioCierre);
        Assert.Equal(44.80m, cerrada.Total);
        Assert.Equal(EstadoMesa.Disponible, db.Mesas.Find(mesa.IdMesa)!.Estado);
    }
}
