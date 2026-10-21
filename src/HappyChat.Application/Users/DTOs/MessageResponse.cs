namespace HappyChat.Application.Users.DTOs;

/// <summary>Resposta genérica com uma mensagem de texto.</summary>
/// <param name="Message">Mensagem para exibir ao usuário.</param>
public record MessageResponse(string Message);