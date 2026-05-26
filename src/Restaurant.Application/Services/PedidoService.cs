using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Abstractions;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Services;

public record LineaPedidoInput(int ProductoId, short Cantidad, string? Observaciones);

public interface IPedidoService
{
    Task<Cuenta?> ObtenerCuentaDetalleAsync(int cuentaId);
    Task<List<Producto>> MenuDisponibleAsync();
    Task<OperationResult> RegistrarPedidoAsync(int cuentaId, int idUsuarioMesero,
        IEnumerable<LineaPedidoInput> lineas, string? observacionGeneral);
    Task<OperationResult> EnviarACocinaAsync(int pedidoId, int idUsuario);
}

public class PedidoService : IPedidoService
{
    private readonly IAppDbContext _db;

    public PedidoService(IAppDbContext db) => _db = db;

    public Task<Cuenta?> ObtenerCuentaDetalleAsync(int cuentaId) =>
        _db.Cuentas
            .Include(c => c.Mesa)
            .Include(c => c.Pedidos.OrderBy(p => p.FechaCreacion))
                .ThenInclude(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCuenta == cuentaId);

    public Task<List<Producto>> MenuDisponibleAsync() =>
        _db.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Disponible && p.Categoria.Activa)
            .OrderBy(p => p.Categoria.Nombre).ThenBy(p => p.Nombre)
            .AsNoTracking()
            .ToListAsync();

    public async Task<OperationResult> RegistrarPedidoAsync(int cuentaId, int idUsuarioMesero,
        IEnumerable<LineaPedidoInput> lineas, string? observacionGeneral)
    {
        var cuenta = await _db.Cuentas.FirstOrDefaultAsync(c => c.IdCuenta == cuentaId);
        if (cuenta is null)
            return OperationResult.Fail("La cuenta no existe.");

        // HU-06 / RNF-13: el pedido debe asociarse a una mesa (cuenta) abierta.
        if (cuenta.Estado != EstadoCuenta.Abierta)
            return OperationResult.Fail("No se pueden registrar pedidos en una cuenta cerrada.");

        var items = lineas.Where(l => l.Cantidad > 0).ToList();
        if (items.Count == 0)
            return OperationResult.Fail("Seleccione al menos un producto con cantidad mayor a cero.");

        var ids = items.Select(i => i.ProductoId).ToList();
        var productos = await _db.Productos
            .Where(p => ids.Contains(p.IdProducto))
            .ToDictionaryAsync(p => p.IdProducto);

        var pedido = new Pedido
        {
            IdCuenta = cuenta.IdCuenta,
            IdUsuarioMesero = idUsuarioMesero,
            FechaCreacion = DateTime.Now,
            Estado = EstadoPedido.Pendiente,
            ObservacionGeneral = string.IsNullOrWhiteSpace(observacionGeneral) ? null : observacionGeneral.Trim()
        };

        foreach (var item in items)
        {
            if (!productos.TryGetValue(item.ProductoId, out var prod) || !prod.Disponible)
                return OperationResult.Fail("Uno de los productos no esta disponible.");

            pedido.Detalles.Add(new PedidoDetalle
            {
                IdProducto = prod.IdProducto,
                Cantidad = item.Cantidad,
                PrecioUnitario = prod.Precio,
                Subtotal = prod.Precio * item.Cantidad,
                Observaciones = string.IsNullOrWhiteSpace(item.Observaciones) ? null : item.Observaciones.Trim()
            });
        }

        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();
        return OperationResult.Success("Pedido registrado.", pedido.IdPedido);
    }

    public async Task<OperationResult> EnviarACocinaAsync(int pedidoId, int idUsuario)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.IdPedido == pedidoId);
        if (pedido is null)
            return OperationResult.Fail("El pedido no existe.");

        if (pedido.Estado != EstadoPedido.Pendiente)
            return OperationResult.Fail("Solo se pueden enviar pedidos pendientes.");

        // HU-08: el pedido enviado no debe duplicarse en cocina.
        if (pedido.EnviadoACocina)
            return OperationResult.Fail("El pedido ya fue enviado a cocina.");

        pedido.FechaEnvioCocina = DateTime.Now;
        await _db.SaveChangesAsync();
        return OperationResult.Success("Pedido enviado a cocina.");
    }
}
