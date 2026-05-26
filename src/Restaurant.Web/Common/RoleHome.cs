using Restaurant.Domain.Enums;

namespace Restaurant.Web.Common;

/// <summary>
/// Resuelve la vista inicial segun el rol del usuario (HU-01: el sistema
/// debe dirigir al usuario a la vista correspondiente segun su rol).
/// </summary>
public static class RoleHome
{
    public static (string Controller, string Action) For(string? rol) => rol switch
    {
        RolNombre.Cocina => ("Cocina", "Index"),
        _ => ("Mesas", "Index")
    };
}
