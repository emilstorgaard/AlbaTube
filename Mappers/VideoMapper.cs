using AlbaTube.Dtos.Response;
using AlbaTube.Models;

namespace AlbaTube.Mappers;

public static class VideoMapper
{
    public static VideoRespDto MapToDto(Video video, int likeCount, bool isLiked)
    {
        return new VideoRespDto
        {
            Id = video.Id,
            Title = video.Title,
            Description = video.Description,
            Duration = video.Duration,
            VideoPath = video.VideoPath,
            ThumbnailPath = video.ThumbnailPath,
            UserId = video.UserId,
            ViewCount = video.ViewCount,
            LikeCount = likeCount,
            IsLiked = isLiked,
            CreatedAtUtc = video.CreatedAtUtc,
            UpdatedAtUtc = video.UpdatedAtUtc
        };
    }
}
