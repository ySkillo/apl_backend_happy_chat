using HappyChat.Application.Users.DTOs;

namespace HappyChat.Application.Users.Interfaces;

public interface IFollowService
{
    Task FollowAsync(long followerId, long followedId);
    Task UnfollowAsync(long followerId, long followedId);
    Task<PagedResponse<FollowUserResponse>> GetFollowersAsync(long userId, int page, int pageSize);
    Task<PagedResponse<FollowUserResponse>> GetFollowingAsync(long userId, int page, int pageSize);
}