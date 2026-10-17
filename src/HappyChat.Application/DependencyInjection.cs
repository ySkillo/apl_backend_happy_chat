using HappyChat.Application.Auth.Interfaces;
using HappyChat.Application.Auth.Services;
using HappyChat.Application.Users.Interfaces;
using HappyChat.Application.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HappyChat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
