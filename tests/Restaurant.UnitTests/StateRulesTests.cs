using Restaurant.Application.Services;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Xunit;

namespace Restaurant.UnitTests;

public class StateRulesTests
{
    [Fact]
    public async Task Abrir_mesa_solo_si_disponible()
    {
        using var db = TestDb.Create();
        var (mesa, mesero, _) = TestDb.SeedBase(db);
        var sut = new MesaService(db);

        var primera = await sut.AbrirMesaAsync(mesa.IdMesa, mesero.IdUsuario);
        Assert.True(primera.Ok);
        Assert.Equal(EstadoMesa.Ocupada, db.Mesas.Find(mesa.IdMesa)!.Estado);
        Assert.Equal(EstadoCuenta.Abierta, db.Cuentas.Find(primera.Id)!.Estado);

        // Ya esta ocupada -> no debe poder abrirse de nuevo.
        var segunda = await sut.AbrirMesaAsync(mesa.IdMesa, mesero.IdUsuario);
        Assert.False(segunda.Ok);
    }

    [Fact]
    public async Task Enviar_a_cocina_no_se_duplica()
    {
        using var db = TestDb.Create();
        var (mesa, mesero, prod) = TestDb.SeedBase(db);
        var cuenta = new Cuenta { Mesa = mesa, UsuarioApertura = mesero, Estado = EstadoCuenta.Abierta, FechaApertura = DateTime.Now };
        db.Cuentas.Add(cuenta); db.SaveChanges();
        var pedido = new Pedido { IdCuenta = cuenta.IdCuenta, UsuarioMesero = mesero, Estado = EstadoPedido.Pendiente, FechaCreacion = DateTime.Now };
        pedido.Detalles.Add(new PedidoDetalle { IdProducto = prod.IdProducto, Cantidad = 1, PrecioUnitario = prod.Precio, Subtotal = prod.Precio });
        db.Pedidos.Add(pedido); db.SaveChanges();

        var sut = new PedidoService(db);
        Assert.True((await sut.EnviarACocinaAsync(pedido.IdPedido, mesero.IdUsuario)).Ok);
        Assert.False((await sut.EnviarACocinaAsync(pedido.IdPedido, mesero.IdUsuario)).Ok);
    }

    [Fact]
    public async Task Cocina_transiciones_pendiente_preparacion_listo()
    {
        using var db = TestDb.Create();
        var (mesa, mesero, prod) = TestDb.SeedBase(db);
        var cuenta = new Cuenta { Mesa = mesa, UsuarioApertura = mesero, Estado = EstadoCuenta.Abierta, FechaApertura = DateTime.Now };
        db.Cuentas.Add(cuenta); db.SaveChanges();
        var pedido = new Pedido
        {
            IdCuenta = cuenta.IdCuenta, UsuarioMesero = mesero,
            Estado = EstadoPedido.Pendiente, FechaCreacion = DateTime.Now,
            FechaEnvioCocina = DateTime.Now
        };
        pedido.Detalles.Add(new PedidoDetalle { IdProducto = prod.IdProducto, Cantidad = 1, PrecioUnitario = prod.Precio, Subtotal = prod.Precio });
        db.Pedidos.Add(pedido); db.SaveChanges();

        var sut = new CocinaService(db);

        Assert.True((await sut.IniciarPreparacionAsync(pedido.IdPedido)).Ok);
        Assert.Equal(EstadoPedido.EnPreparacion, db.Pedidos.Find(pedido.IdPedido)!.Estado);

        Assert.True((await sut.MarcarListoAsync(pedido.IdPedido)).Ok);
        var listo = db.Pedidos.Find(pedido.IdPedido)!;
        Assert.Equal(EstadoPedido.Listo, listo.Estado);
        Assert.NotNull(listo.FechaListo);

        // No se puede marcar listo dos veces.
        Assert.False((await sut.MarcarListoAsync(pedido.IdPedido)).Ok);
    }
}
