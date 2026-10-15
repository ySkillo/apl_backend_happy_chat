using HappyChat.Application.Interfaces;
using HappyChat.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HappyChat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
