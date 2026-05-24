using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Services;

namespace Restaurant.Application;

public static class DependencyInjection
{
    /// <summary>Registra los servicios de la capa de aplicacion.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMesaService, MesaService>();
        services.AddScoped<IPedidoService, PedidoService>();
        return services;
    }
}
