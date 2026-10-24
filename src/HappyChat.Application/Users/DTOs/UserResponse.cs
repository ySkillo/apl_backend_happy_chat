namespace HappyChat.Application.Users.DTOs;
/// <summary>Dados públicos de um usuário.</summary>
public class UserResponse
{
    /// <summary>Identificador único do usuário.</summary>
    /// <example>1</example>
    public long Id { get; set; }

    /// <summary>Nome de exibição.</summary>
    /// <example>Maria Silva</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>Identificador público único (@perfil), sempre em minúsculas.</summary>
    /// <example>mari_ofc</example>
    public string Perfil { get; set; } = string.Empty;

    /// <summary>E-mail da conta, sempre em minúsculas.</summary>
    /// <example>maria@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>Indica se a conta está ativa. Contas inativas não conseguem fazer login.</summary>
    public bool IsActive { get; set; }

    /// <summary>Indica se o usuário está com a sessão aberta (true após o login, false após o logout).</summary>
    public bool IsOnline { get; set; }

    /// <summary>Data e hora (UTC) de criação da conta.</summary>
    public DateTime CreatedAt { get; set; }
}
