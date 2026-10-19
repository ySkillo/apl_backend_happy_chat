namespace HappyChat.Domain.Users.Entities;

public class PendingRegistration
{
    public string Name { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int Attempts { get; set; }
}