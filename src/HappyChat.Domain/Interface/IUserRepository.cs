using HappyChat.Domain.Entities;

namespace HappyChat.Domain.Interface;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPerfilAsync(string perfil);
    Task<IReadOnlyList<User>> GetAllAsync();
}
