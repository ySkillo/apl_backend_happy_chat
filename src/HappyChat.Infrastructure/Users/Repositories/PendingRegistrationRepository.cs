using HappyChat.Domain.Users.Entities;
using HappyChat.Domain.Users.Interfaces;
namespace HappyChat.Infrastructure.Users.Repositories;

public class PendingRegistrationRepository : IPendingRegistrationRepository
{
    private readonly Dictionary<string, PendingRegistration> _items =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public Task SaveAsync(PendingRegistration pending)
    {
        lock (_lock) { _items[pending.Email] = pending; }
        return Task.CompletedTask;
    }

    public Task<PendingRegistration?> GetByEmailAsync(string email)
    {
        lock (_lock)
        {
            _items.TryGetValue(email, out var pending);
            return Task.FromResult(pending);
        }
    }

    public Task<int> RegisterFailedAttemptAsync(string email)
    {
        lock (_lock)
        {
            if (!_items.TryGetValue(email, out var pending)) return Task.FromResult(0);
            pending.Attempts++;
            return Task.FromResult(pending.Attempts);
        }
    }

    public Task RemoveAsync(string email)
    {
        lock (_lock) { _items.Remove(email); }
        return Task.CompletedTask;
    }
}