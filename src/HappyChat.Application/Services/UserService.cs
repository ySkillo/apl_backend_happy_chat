using HappyChat.Application.DTOs;
using HappyChat.Application.Interfaces;
using HappyChat.Domain.Entities;
using HappyChat.Domain.Exceptions;
using HappyChat.Domain.Interface;

namespace HappyChat.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> RegisterAsync(CreateUserRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Checa antes de gerar o hash, que é uma operação cara.
        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new EmailAlreadyRegisteredException();

        var now = DateTime.UtcNow;
        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userRepository.AddAsync(user);

        return ToResponse(user);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : ToResponse(user);
    }
    
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(ToResponse).ToList();
    }

    private static UserResponse ToResponse(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}
