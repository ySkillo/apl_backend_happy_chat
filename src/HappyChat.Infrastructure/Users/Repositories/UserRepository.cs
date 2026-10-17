using HappyChat.Domain.Users.Entities;
using HappyChat.Domain.Users.Exceptions;
using HappyChat.Domain.Users.Interfaces;

namespace HappyChat.Infrastructure.Users.Repositories;

/// <summary>
/// Implementação em memória (uma List). Os dados somem quando a API reinicia.
/// Quando entrar o EF Core, só esta classe é trocada; o resto da aplicação não muda.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    // O repositório é singleton e a API atende várias requisições ao mesmo tempo,
    // então o acesso à lista precisa ser protegido.
    private readonly object _lock = new();

    public Task AddAsync(User user)
    {
        lock (_lock)
        {
            // Faz o papel de um índice único no banco: garante e-mail único
            // mesmo se duas requisições chegarem ao mesmo tempo.
            if (_users.Any(u => IsSameEmail(u.Email, user.Email)))
                throw new EmailAlreadyRegisteredException();

            _users.Add(user);
        }

        return Task.CompletedTask;
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        lock (_lock)
        {
            return Task.FromResult(_users.FirstOrDefault(u => IsSameEmail(u.Email, email)));
        }
    }

    private static bool IsSameEmail(string a, string b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    
    public Task<IReadOnlyList<User>> GetAllAsync()
    {
        lock (_lock)
        {
            // Devolve uma cópia: quem chama não enxerga a lista interna
            // nem sofre com cadastros acontecendo durante a leitura.
            return Task.FromResult<IReadOnlyList<User>>(_users.ToList());
        }
    }

    public Task<User?> GetByPerfilAsync(string perfil)
    {
        lock (_lock)
        {
            return Task.FromResult(_users.FirstOrDefault(u => IsSamePerfil(u.Perfil, perfil)));
        }
    }
    
    private static bool IsSamePerfil(string a, string b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
