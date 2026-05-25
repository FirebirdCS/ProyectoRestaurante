using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Abstractions;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Services;

public interface ICocinaService
{
    Task<List<Pedido>> ColaAsync();
    Task<OperationResult> IniciarPreparacionAsync(int pedidoId);
    Task<OperationResult> MarcarListoAsync(int pedidoId);
}

public class CocinaService : ICocinaService
{
    private readonly IAppDbContext _db;

    public CocinaService(IAppDbContext db) => _db = db;

    // HU-10: solo pedidos enviados, activos, por orden de llegada a cocina.
    public Task<List<Pedido>> ColaAsync() =>
        _db.Pedidos
            .Include(p => p.Cuenta).ThenInclude(c => c.Mesa)
            .Include(p => p.Detalles).ThenInclude(d => d.Producto)
            .Where(p => p.FechaEnvioCocina != null
                        && (p.Estado == EstadoPedido.Pendiente || p.Estado == EstadoPedido.EnPreparacion))
            .OrderBy(p => p.FechaEnvioCocina)
            .AsNoTracking()
            .ToListAsync();

    // HU-11: pendiente -> en preparacion.
    public async Task<OperationResult> IniciarPreparacionAsync(int pedidoId)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.IdPedido == pedidoId);
        if (pedido is null)
            return OperationResult.Fail("El pedido no existe.");
        if (pedido.Estado != EstadoPedido.Pendiente || !pedido.EnviadoACocina)
            return OperationResult.Fail("El pedido no esta pendiente de preparacion.");

        pedido.Estado = EstadoPedido.EnPreparacion;
        await _db.SaveChangesAsync();
        return OperationResult.Success("Pedido en preparacion.");
    }

    // HU-11: en preparacion -> listo (registra fecha/hora).
    public async Task<OperationResult> MarcarListoAsync(int pedidoId)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.IdPedido == pedidoId);
        if (pedido is null)
            return OperationResult.Fail("El pedido no existe.");
        if (pedido.Estado != EstadoPedido.EnPreparacion)
            return OperationResult.Fail("Solo pedidos en preparacion pueden marcarse como listos.");

        pedido.Estado = EstadoPedido.Listo;
        pedido.FechaListo = DateTime.Now;
        await _db.SaveChangesAsync();
        return OperationResult.Success("Pedido listo para entregar.");
    }
}
