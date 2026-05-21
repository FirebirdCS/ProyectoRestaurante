namespace Restaurant.Domain.Entities;

/// <summary>Categoria a la que pertenece un producto del menu.</summary>
public class CategoriaProducto
{
  public int IdCategoria { get; set; }
  public string Nombre { get; set; } = null!;
  public string? Descripcion { get; set; }
  public bool Activa { get; set; } = true;

  public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
