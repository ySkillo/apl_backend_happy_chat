using System.ComponentModel.DataAnnotations;

namespace HappyChat.Application.Auth.DTOs;
/// <summary>Credenciais para autenticação.</summary>
public class LoginRequest
{
    /// <summary>E-mail cadastrado.</summary>
    /// <example>maria@email.com</example>
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Senha da conta.</summary>
    /// <example>senh@Forte123</example>
    [Required]
    [StringLength(128)]
    public string Password { get; set; } = string.Empty;
}