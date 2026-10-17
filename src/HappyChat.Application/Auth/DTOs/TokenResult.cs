namespace HappyChat.Application.Auth.DTOs;

public record TokenResult(string AccessToken, DateTime ExpiresAt);
