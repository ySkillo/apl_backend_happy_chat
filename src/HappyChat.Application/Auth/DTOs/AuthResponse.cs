using HappyChat.Application.Users.DTOs;

namespace HappyChat.Application.Auth.DTOs;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = new();
}
