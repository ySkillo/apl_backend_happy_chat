using HappyChat.Domain.Common;
using HappyChat.Domain.Users.Entities;
using HappyChat.Domain.Users.Exceptions;
using HappyChat.Domain.Users.Interfaces;
using HappyChat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HappyChat.Infrastructure.Users.Repositories;

public class FollowRepository(AppDbContext db) : IFollowRepository
{
    public async Task AddAsync(Follow follow)
    {
        db.Follows.Add(follow);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new AlreadyFollowingException();
        }
    }

    public Task<Follow?> GetAsync(long followerId, long followedId) =>
        db.Follows.AsNoTracking()
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);

    public async Task DeleteAsync(Follow follow)
    {
        db.Follows.Remove(follow);
        await db.SaveChangesAsync();
    }
    
    public Task<PagedResult<Follow>> GetFollowersAsync(long userId, int skip, int take) =>
        PageAsync(db.Follows.Where(f => f.FollowedId == userId).Include(f => f.Follower), skip, take);

    public Task<PagedResult<Follow>> GetFollowingAsync(long userId, int skip, int take) =>
        PageAsync(db.Follows.Where(f => f.FollowerId == userId).Include(f => f.Followed), skip, take);

    private static async Task<PagedResult<Follow>> PageAsync(IQueryable<Follow> query, int skip, int take)
    {
        query = query.AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .ThenBy(f => f.FollowerId)
            .ThenBy(f => f.FollowedId)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new PagedResult<Follow>(items, total);
    }
}