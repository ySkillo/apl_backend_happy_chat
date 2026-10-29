using HappyChat.Application.Users.DTOs;
using HappyChat.Domain.Users.Entities;

namespace HappyChat.Application.Users.Mappings;

public static class FollowMappings
{
    /// <summary>Para a lista de seguidores: o usuário exibido é quem segue.</summary>
    public static FollowUserResponse ToFollowerResponse(this Follow follow) =>
        ToResponse(follow.Follower, follow.CreatedAt);

    /// <summary>Para a lista de seguidos: o usuário exibido é quem é seguido.</summary>
    public static FollowUserResponse ToFollowingResponse(this Follow follow) =>
        ToResponse(follow.Followed, follow.CreatedAt);

    private static FollowUserResponse ToResponse(User user, DateTime followedAtUtc) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Perfil = user.Perfil,
        IsOnline = user.IsOnline,
        FollowedAt = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(followedAtUtc, DateTimeKind.Utc),
            TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"))
    };
}