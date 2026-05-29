namespace Restaurant.Domain.Entities;

/// <summary>Rol del sistema (administrador, mesero, cocina, cajero).</summary>
public class Rol
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
