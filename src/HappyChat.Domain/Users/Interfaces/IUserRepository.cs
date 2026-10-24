using HappyChat.Domain.Users.Entities;

namespace HappyChat.Domain.Users.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPerfilAsync(string perfil);
    Task<IReadOnlyList<User>> GetAllAsync();
    /// <summary>Atualiza só o status online do usuário. Devolve false se o usuário não existe.</summary>
    Task<bool> SetOnlineAsync(long id, bool isOnline);
}
