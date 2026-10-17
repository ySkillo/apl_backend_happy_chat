using HappyChat.Application.Auth.DTOs;
using HappyChat.Domain.Users.Entities;

namespace HappyChat.Application.Auth.Interfaces;

public interface ITokenService
{
    TokenResult GenerateToken(User user);
}
