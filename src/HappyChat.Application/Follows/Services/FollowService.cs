using HappyChat.Application.Common.DTOs;
using HappyChat.Application.Follows.DTOs;
using HappyChat.Application.Follows.Interfaces;
using HappyChat.Application.Follows.Mappings;
using HappyChat.Application.Users.Interfaces;
using HappyChat.Domain.Common;
using HappyChat.Domain.Follows.Entities;
using HappyChat.Domain.Follows.Exceptions;
using HappyChat.Domain.Follows.Interfaces;
using HappyChat.Domain.Users.Exceptions;
using HappyChat.Domain.Users.Interfaces;

namespace HappyChat.Application.Follows.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailNotifier _emailNotifier;
    private const int MaxPageSize = 50;

    public FollowService(IFollowRepository followRepository, IUserRepository userRepository, IEmailNotifier emailNotifier)
    {
        _followRepository = followRepository;
        _userRepository = userRepository;
        _emailNotifier =  emailNotifier;
    }

    public async Task FollowAsync(long followerId, long followedId)
    {
        if (followerId == followedId)
            throw new FollowingOneselfException();
        
        var followed = await _userRepository.GetByIdAsync(followedId)
                       ?? throw new UserNotFoundException();

        var follower = await _userRepository.GetByIdAsync(followerId)
                       ?? throw new UserNotFoundException();
        
        if (await _followRepository.GetAsync(followerId, followedId) is not null)
            throw new AlreadyFollowingException();

        await _followRepository.AddAsync(new Follow
        {
            FollowerId = followerId,
            FollowedId = followedId
        });

        await _emailNotifier.SendNewFollowerAsync(
            followed.Email, followed.Name,follower.Name, follower.Perfil
            );
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