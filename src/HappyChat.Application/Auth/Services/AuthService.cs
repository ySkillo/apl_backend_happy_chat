using HappyChat.Application.Auth.DTOs;
using HappyChat.Application.Auth.Exceptions;
using HappyChat.Application.Auth.Interfaces;
using HappyChat.Application.Users.Mappings;
using HappyChat.Domain.Users.Interfaces;

namespace HappyChat.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            // Gasta o mesmo tempo de uma verificação real, para não revelar
            // pelo tempo de resposta se o e-mail existe ou não.
            _passwordHasher.Hash(request.Password);
            throw new InvalidCredentialsException();
        }

        var passwordOk = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!passwordOk || !user.IsActive)
            throw new InvalidCredentialsException();

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            AccessToken = token.AccessToken,
            ExpiresAt = token.ExpiresAt,
            User = user.ToResponse()
        };
    }
}
