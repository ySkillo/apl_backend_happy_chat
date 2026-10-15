using HappyChat.Application.DTOs;

namespace HappyChat.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
