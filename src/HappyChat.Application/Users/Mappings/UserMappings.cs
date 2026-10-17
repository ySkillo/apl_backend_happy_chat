using HappyChat.Application.Users.DTOs;
using HappyChat.Domain.Users.Entities;

namespace HappyChat.Application.Users.Mappings;

public static class UserMappings
{   
    
    public static UserResponse ToResponse(this User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Perfil =  user.Perfil,
        Email = user.Email,
        IsActive = user.IsActive,
        CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc),
            TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo")
        )
    };
}
