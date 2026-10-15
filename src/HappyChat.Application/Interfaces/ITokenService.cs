using HappyChat.Application.DTOs;
using HappyChat.Domain.Entities;

namespace HappyChat.Application.Interfaces;

public interface ITokenService
{
    TokenResult GenerateToken(User user);
}
