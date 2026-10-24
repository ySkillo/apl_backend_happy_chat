using HappyChat.Application.Auth.DTOs;

namespace HappyChat.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync(long userId);
}
