using HappyChat.Application.Auth.Interfaces;
using HappyChat.Domain.Users.Interfaces;
using HappyChat.Infrastructure.Auth.Services;
using HappyChat.Infrastructure.Auth.Settings;
using HappyChat.Infrastructure.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HappyChat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, JwtSettings jwtSettings)
    {
        // Singleton porque a lista em memória precisa sobreviver entre requisições.
        // Com EF Core (DbContext), o repositório passa a ser Scoped.
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddSingleton(jwtSettings);
        services.AddSingleton<ITokenService, TokenService>();
        return services;
    }
}
