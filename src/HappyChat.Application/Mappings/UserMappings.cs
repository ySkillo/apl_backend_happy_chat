using HappyChat.Application.DTOs;
using HappyChat.Domain.Entities;

namespace HappyChat.Application.Mappings;

public static class UserMappings
{
    public static UserResponse ToResponse(this User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}
