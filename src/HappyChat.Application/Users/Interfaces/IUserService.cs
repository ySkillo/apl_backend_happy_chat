using HappyChat.Application.Users.DTOs;

namespace HappyChat.Application.Users.Interfaces;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(CreateUserRequest request);
    Task<UserResponse?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<UserResponse>> GetAllAsync();
}
