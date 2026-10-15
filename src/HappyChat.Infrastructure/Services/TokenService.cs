using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HappyChat.Application.DTOs;
using HappyChat.Application.Interfaces;
using HappyChat.Domain.Entities;
using HappyChat.Infrastructure.Settings;
using Microsoft.IdentityModel.Tokens;

namespace HappyChat.Infrastructure.Services;

/// <summary>Gera o JWT (assinado com HMAC SHA-256) de um usuário autenticado.</summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(JwtSettings settings)
    {
        _settings = settings;
    }

    public TokenResult GenerateToken(User user)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_settings.ExpireMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
