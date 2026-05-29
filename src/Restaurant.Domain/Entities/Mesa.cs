using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities;

/// <summary>Mesa fisica del restaurante y su estado operativo.</summary>
public class Mesa
{
    public int IdMesa { get; set; }
    public int NumeroMesa { get; set; }
    public byte Capacidad { get; set; }
    public string Estado { get; set; } = EstadoMesa.Disponible;
    public string? Ubicacion { get; set; }
    public bool Activa { get; set; } = true;

    public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}
