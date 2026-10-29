namespace HappyChat.Application.Users.DTOs;

/// <summary>Usuário em uma lista de seguidores/seguidos.</summary>
public class FollowUserResponse
{
    /// <summary>Identificador do usuário.</summary>
    /// <example>2</example>
    public long Id { get; set; }

    /// <summary>Nome de exibição.</summary>
    /// <example>Maria Silva</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>Identificador público único (@perfil).</summary>
    /// <example>mari_ofc</example>
    public string Perfil { get; set; } = string.Empty;

    /// <summary>Indica se o usuário está online.</summary>
    public bool IsOnline { get; set; }

    /// <summary>Data e hora em que o follow aconteceu (horário de Brasília).</summary>
    public DateTime FollowedAt { get; set; }
}