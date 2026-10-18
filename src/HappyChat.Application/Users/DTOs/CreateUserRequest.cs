using System.ComponentModel.DataAnnotations;

namespace HappyChat.Application.Users.DTOs;

public class CreateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(15, MinimumLength = 3)]
    public string Perfil { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
