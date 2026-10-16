using HappyChat.Application.DTOs;
using HappyChat.Domain.Entities;

namespace HappyChat.Application.Mappings;

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
