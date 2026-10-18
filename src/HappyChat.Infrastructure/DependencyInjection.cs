using HappyChat.Application.Auth.Interfaces;
using HappyChat.Application.Users.Interfaces;
using HappyChat.Domain.Users.Interfaces;
using HappyChat.Infrastructure.Auth.Services;
using HappyChat.Infrastructure.Auth.Settings;
using HappyChat.Infrastructure.Email;
using HappyChat.Infrastructure.Users.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HappyChat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, JwtSettings jwtSettings,  IConfiguration configuration)
    {
        // Singleton porque a lista em memória precisa sobreviver entre requisições.
        // Com EF Core (DbContext), o repositório passa a ser Scoped.
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddSingleton(jwtSettings);
        services.AddSingleton<ITokenService, TokenService>();
        
        var baseUrl = configuration["EmailService:BaseUrl"]
                      ?? throw new InvalidOperationException("Configuração 'EmailService:BaseUrl' não encontrada no appsettings.");

        services.AddHttpClient<IEmailNotifier, EmailServiceClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        return services;
    }
}
