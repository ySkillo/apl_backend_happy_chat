using HappyChat.Domain.Common;
using HappyChat.Domain.Users.Entities;

namespace HappyChat.Domain.Users.Interfaces;

public interface IFollowRepository
{
    Task AddAsync(Follow follow);
    Task<Follow?> GetAsync(long followerId, long followedId);
    Task DeleteAsync(Follow follow);
    
    /// <summary>Quem segue o usuário. Cada Follow vem com a navegação Follower carregada.</summary>
    Task<PagedResult<Follow>> GetFollowersAsync(long userId, int skip, int take);

    /// <summary>Quem o usuário segue. Cada Follow vem com a navegação Followed carregada.</summary>
    Task<PagedResult<Follow>> GetFollowingAsync(long userId, int skip, int take);
}