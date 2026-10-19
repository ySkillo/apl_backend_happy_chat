using System.Security.Cryptography;
using System.Text;
using HappyChat.Application.Auth.Interfaces;
using HappyChat.Application.Users.DTOs;
using HappyChat.Application.Users.Interfaces;
using HappyChat.Application.Users.Mappings;
using HappyChat.Domain.Users.Entities;
using HappyChat.Domain.Users.Exceptions;
using HappyChat.Domain.Users.Interfaces;

namespace HappyChat.Application.Users.Services;

public class UserService : IUserService
{
    private const int CodeValidityMinutes = 10;
    private const int MaxAttempts = 5;

    private readonly IUserRepository _userRepository;
    private readonly IPendingRegistrationRepository _pendingRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailNotifier _emailNotifier;

    public UserService(
        IUserRepository userRepository,
        IPendingRegistrationRepository pendingRepository,
        IPasswordHasher passwordHasher,
        IEmailNotifier emailNotifier)
    {
        _userRepository = userRepository;
        _pendingRepository = pendingRepository;
        _passwordHasher = passwordHasher;
        _emailNotifier = emailNotifier;
    }
    
    public async Task StartRegistrationAsync(CreateUserRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var perfil = request.Perfil.Trim().ToLowerInvariant();

        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new EmailAlreadyRegisteredException();

        if (await _userRepository.GetByPerfilAsync(perfil) is not null)
            throw new PerfilAlreadyRegisteredException();

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var name = request.Name.Trim();

        await _pendingRepository.SaveAsync(new PendingRegistration
        {
            Name = name,
            Perfil = perfil,
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CodeHash = HashCode(email, code),
            ExpiresAt = DateTime.UtcNow.AddMinutes(CodeValidityMinutes),
            Attempts = 0
        });

        try
        {
            await _emailNotifier.SendVerificationCodeAsync(email, name, code);
        }
        catch
        {
            await _pendingRepository.RemoveAsync(email);
            throw;
        }
    }
    
    public async Task<UserResponse> ConfirmRegistrationAsync(VerifyEmailRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var pending = await _pendingRepository.GetByEmailAsync(email);

        if (pending is null)
            throw new VerificationCodeExpiredException();

        if (pending.ExpiresAt < DateTime.UtcNow)
        {
            await _pendingRepository.RemoveAsync(email);
            throw new VerificationCodeExpiredException();
        }

        if (pending.Attempts >= MaxAttempts)
        {
            await _pendingRepository.RemoveAsync(email);
            throw new TooManyVerificationAttemptsException();
        }

        if (!CodeMatches(email, request.Code, pending.CodeHash))
        {
            var attempts = await _pendingRepository.RegisterFailedAttemptAsync(email);
            if (attempts >= MaxAttempts)
            {
                await _pendingRepository.RemoveAsync(email);
                throw new TooManyVerificationAttemptsException();
            }
            throw new InvalidVerificationCodeException();
        }
        
        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new EmailAlreadyRegisteredException();
        if (await _userRepository.GetByPerfilAsync(pending.Perfil) is not null)
            throw new PerfilAlreadyRegisteredException();

        var now = DateTime.UtcNow;
        var user = new User
        {
            Name = pending.Name,
            Perfil = pending.Perfil,
            Email = pending.Email,
            PasswordHash = pending.PasswordHash,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userRepository.AddAsync(user);
        await _pendingRepository.RemoveAsync(email);
        await _emailNotifier.SendAccountOpeningAsync(user.Email, user.Name);
        return user.ToResponse();
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : user.ToResponse();
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.ToResponse()).ToList();
    }
    
    private static string HashCode(string email, string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{email}:{code}")));

    private static bool CodeMatches(string email, string code, string expectedHash)
    {
        var actual = Encoding.UTF8.GetBytes(HashCode(email, code));
        var expected = Encoding.UTF8.GetBytes(expectedHash);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}