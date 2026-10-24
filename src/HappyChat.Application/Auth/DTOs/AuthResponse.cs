using HappyChat.Application.Users.DTOs;

namespace HappyChat.Application.Auth.DTOs;
/// <summary>Resultado do login: token de acesso e dados do usuário.</summary>
public class AuthResponse
{
    /// <summary>Token JWT para o header Authorization.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Tipo do token; sempre "Bearer".</summary>
    /// <example>Bearer</example>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>Data e hora (UTC) em que o token expira.</summary>
    /// <example>2026-10-21T15:30:00Z</example>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Dados do usuário autenticado.</summary>
    public UserResponse User { get; set; } = new();
}