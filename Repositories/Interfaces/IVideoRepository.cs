using AlbaTube.Models;

namespace AlbaTube.Repositories.Interfaces;

public interface IVideoRepository
{
    Task<Video?> GetVideoById(int id);
    Task IncrementViewCount(int videoId);
    Task<List<Video>> GetVideosByUserId(int userId);
    Task<Video?> GetExsistingVideo(string title);
    Task AddVideo(Video video);
    Task<LikedVideo?> GetLikedVideoByUser(int videoId, int userId);
    Task<List<int>> GetLikedVideoIdsByUser(int userId);
    Task<bool> IsVideoLikedByUser(int videoId, int userId);
    Task LikeVideo(LikedVideo likedVideo);
    Task DislikeVideo(LikedVideo likedVideo);
    Task UpdateVideo(Video video);
    Task DeleteVideo(Video video);
    Task DeleteLikedVideos(int userId);
    Task<int> GetLikeCount(int videoId);
}