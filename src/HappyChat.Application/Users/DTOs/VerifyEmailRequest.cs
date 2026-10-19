using System.ComponentModel.DataAnnotations;

namespace HappyChat.Application.Users.DTOs;

public class VerifyEmailRequest
{
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "O código deve ter 6 dígitos.")]
    public string Code { get; set; } = string.Empty;
}