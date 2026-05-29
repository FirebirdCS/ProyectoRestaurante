namespace Restaurant.Domain.Entities;

/// <summary>Linea de un pedido: producto, cantidad y subtotal.</summary>
public class PedidoDetalle
{
    public int IdPedidoDetalle { get; set; }
    public int IdPedido { get; set; }
    public int IdProducto { get; set; }
    public short Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string? Observaciones { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
