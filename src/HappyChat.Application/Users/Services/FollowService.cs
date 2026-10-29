using HappyChat.Application.Users.DTOs;
using HappyChat.Application.Users.Interfaces;
using HappyChat.Application.Users.Mappings;
using HappyChat.Domain.Common;
using HappyChat.Domain.Users.Entities;
using HappyChat.Domain.Users.Exceptions;
using HappyChat.Domain.Users.Interfaces;

namespace HappyChat.Application.Users.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IUserRepository _userRepository;
    private const int MaxPageSize = 50;

    public FollowService(IFollowRepository followRepository, IUserRepository userRepository)
    {
        _followRepository = followRepository;
        _userRepository = userRepository;
    }

    public async Task FollowAsync(long followerId, long followedId)
    {
        if (followerId == followedId)
            throw new FollowingOneselfException();
        
        if (await _userRepository.GetByIdAsync(followedId) is null)
            throw new UserNotFoundException();
        
        if (await _followRepository.GetAsync(followerId, followedId) is not null)
            throw new AlreadyFollowingException();

        await _followRepository.AddAsync(new Follow
        {
            FollowerId = followerId,
            FollowedId = followedId
        });
    }

    public async Task UnfollowAsync(long followerId, long followedId)
    {
        var follow = await _followRepository.GetAsync(followerId, followedId)
                     ?? throw new NotFollowingException();

        await _followRepository.DeleteAsync(follow);
    }
    
    public async Task<PagedResponse<FollowUserResponse>> GetFollowersAsync(long userId, int page, int pageSize)
    {
        await EnsureUserExistsAsync(userId);
        (page, pageSize) = NormalizePaging(page, pageSize);

        var result = await _followRepository.GetFollowersAsync(userId, (page - 1) * pageSize, pageSize);
        return ToPage(result, page, pageSize, f => f.ToFollowerResponse());
    }

    public async Task<PagedResponse<FollowUserResponse>> GetFollowingAsync(long userId, int page, int pageSize)
    {
        await EnsureUserExistsAsync(userId);
        (page, pageSize) = NormalizePaging(page, pageSize);

        var result = await _followRepository.GetFollowingAsync(userId, (page - 1) * pageSize, pageSize);
        return ToPage(result, page, pageSize, f => f.ToFollowingResponse());
    }

    private async Task EnsureUserExistsAsync(long userId)
    {
        if (await _userRepository.GetByIdAsync(userId) is null)
            throw new UserNotFoundException();
    }

    private static (int Page, int PageSize) NormalizePaging(int page, int pageSize) =>
        (Math.Max(page, 1), Math.Clamp(pageSize, 1, MaxPageSize));

    private static PagedResponse<FollowUserResponse> ToPage(
        PagedResult<Follow> result, int page, int pageSize, Func<Follow, FollowUserResponse> map) => new()
    {
        Items = result.Items.Select(map).ToList(),
        Page = page,
        PageSize = pageSize,
        TotalCount = result.TotalCount
    };
}