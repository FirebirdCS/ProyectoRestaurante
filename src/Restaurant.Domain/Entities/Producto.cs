namespace Restaurant.Domain.Entities;

/// <summary>Producto del menu disponible para los pedidos.</summary>
public class Producto
{
  public int IdProducto { get; set; }
  public int IdCategoria { get; set; }
  public string Nombre { get; set; } = null!;
  public string? Descripcion { get; set; }
  public decimal Precio { get; set; }
  public bool Disponible { get; set; } = true;
  public short? TiempoPreparacionMin { get; set; }

  public CategoriaProducto Categoria { get; set; } = null!;
  public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
}
