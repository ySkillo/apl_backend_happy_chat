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
        var perfil = request.Perfil.Trim().ToLowerInvariant();
        // Checa antes de gerar o hash, que é uma operação cara.
        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new EmailAlreadyRegisteredException();

        if (await _userRepository.GetByPerfilAsync(perfil)  is not null)
            throw new PerfilAlreadyRegisteredException();
        
        var now = DateTime.UtcNow;
        var user = new User
        {
            Name = request.Name.Trim(),
            Perfil = perfil,
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userRepository.AddAsync(user);

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
}
