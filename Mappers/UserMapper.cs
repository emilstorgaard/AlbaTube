using AlbaTube.Dtos.Response;
using AlbaTube.Models;

namespace AlbaTube.Mappers;

public static class UserMapper
{
    public static UserRespDto MapToDto(User user)
    {
        return new UserRespDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            ProfileImageParh = user.ProfileImagePath,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc
        };
    }
}
