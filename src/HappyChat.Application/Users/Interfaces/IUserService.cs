using HappyChat.Application.Users.DTOs;

namespace HappyChat.Application.Users.Interfaces;

public interface IUserService
{
    Task StartRegistrationAsync(CreateUserRequest request);
    Task<UserResponse> ConfirmRegistrationAsync(VerifyEmailRequest request);
    Task<UserResponse?> GetByIdAsync(long id);
    Task<IReadOnlyList<UserResponse>> GetAllAsync();
}
