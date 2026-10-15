using HappyChat.Application.DTOs;

namespace HappyChat.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(CreateUserRequest request);
    Task<UserResponse?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<UserResponse>> GetAllAsync();
}
