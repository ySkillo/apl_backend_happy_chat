using HappyChat.Domain.Users.Entities;
using HappyChat.Domain.Users.Interfaces;
using HappyChat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HappyChat.Infrastructure.Users.Repositories;

public class PendingRegistrationRepository(AppDbContext db) : IPendingRegistrationRepository
{
    public async Task SaveAsync(PendingRegistration pending)
    {
        var existing = await db.PendingRegistrations.FindAsync(pending.Email);
        if (existing is null)
            db.PendingRegistrations.Add(pending);
        else
            db.Entry(existing).CurrentValues.SetValues(pending);
        await db.SaveChangesAsync();
    }

    public Task<PendingRegistration?> GetByEmailAsync(string email) =>
        db.PendingRegistrations.AsNoTracking().FirstOrDefaultAsync(p => p.Email == email.ToLower());

    public async Task<int> RegisterFailedAttemptAsync(string email)
    {
        // Incremento atômico no banco, sem race condition entre requisições.
        var rows = await db.PendingRegistrations
            .Where(p => p.Email == email.ToLower())
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Attempts, p => p.Attempts + 1));
        if (rows == 0) return 0;

        return await db.PendingRegistrations
            .Where(p => p.Email == email.ToLower())
            .Select(p => p.Attempts)
            .FirstAsync();
    }

    public Task RemoveAsync(string email) =>
        db.PendingRegistrations.Where(p => p.Email == email.ToLower()).ExecuteDeleteAsync();
}