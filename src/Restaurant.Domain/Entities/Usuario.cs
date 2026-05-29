namespace Restaurant.Domain.Entities;

/// <summary>Usuario que accede al sistema; pertenece a un unico rol.</summary>
public class Usuario
{
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }

    public Rol Rol { get; set; } = null!;
    public ICollection<Cuenta> CuentasAbiertas { get; set; } = new List<Cuenta>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
