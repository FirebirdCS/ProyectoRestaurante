using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Abstractions;
using Restaurant.Infrastructure.Persistence;
using Restaurant.Infrastructure.Security;

namespace Restaurant.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Registra DbContext, persistencia y servicios de infraestructura.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<RestaurantDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<RestaurantDbContext>());
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        return services;
    }
}
