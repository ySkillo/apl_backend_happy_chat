using System.ComponentModel.DataAnnotations;

namespace HappyChat.Application.Users.DTOs;
/// <summary>Confirmação do cadastro com o código recebido por e-mail.</summary>
public class VerifyEmailRequest
{
    /// <summary>E-mail usado no cadastro.</summary>
    /// <example>maria@email.com</example>
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Código de 6 dígitos recebido por e-mail.</summary>
    /// <example>042917</example>
    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "O código deve ter 6 dígitos.")]
    public string Code { get; set; } = string.Empty;
}