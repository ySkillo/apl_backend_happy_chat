using HappyChat.Domain.Users.Entities;
using HappyChat.Domain.Users.Exceptions;
using HappyChat.Domain.Users.Interfaces;
using HappyChat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HappyChat.Infrastructure.Users.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pg)
        {
            throw pg.ConstraintName switch
            {
                "ix_users_perfil" => new PerfilAlreadyRegisteredException(),
                _ => new EmailAlreadyRegisteredException()
            };
        }
    }

    public Task<User?> GetByIdAsync(long id) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public Task<User?> GetByPerfilAsync(string perfil) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Perfil == perfil.ToLower());

    public async Task<IReadOnlyList<User>> GetAllAsync() =>
        await db.Users.AsNoTracking().ToListAsync();

    public async Task<bool> SetOnlineAsync(long id, bool isOnline) =>
        await db.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsOnline, isOnline)) > 0;
}