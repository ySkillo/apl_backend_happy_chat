using System.ComponentModel.DataAnnotations;

namespace HappyChat.Application.Users.DTOs;
/// <summary>Dados para iniciar o cadastro de um novo usuário.</summary>
public class CreateUserRequest
{
    /// <summary>Nome de exibição do usuário.</summary>
    /// <example>Maria Silva</example>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    /// <summary>Identificador público e único do usuário (o "@"). Salvo em minúsculas.</summary>
    /// <example>mari_ofc</example>
    [Required]
    [StringLength(15, MinimumLength = 3)]
    public string Perfil { get; set; } = string.Empty;
    /// <summary>E-mail único da conta. Recebe o código de verificação e é salvo em minúsculas.</summary>
    /// <example>maria@email.com</example>
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;
    /// <summary>Senha de acesso à conta.</summary>
    /// <example>senh@Forte123</example>
    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
