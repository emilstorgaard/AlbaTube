using AlbaTube.Dtos.Request;
using AlbaTube.Dtos.Response;

namespace AlbaTube.Services.Interfaces;

public interface IVideoService
{
    Task<FileStream> Stream(int id);
    Task<VideoRespDto> GetVideoById(int id, int loggedInUserId);
    Task<List<VideoRespDto>> GetAllVideosByUserId(int userId, int loggedInUserId);
    string GetThumbnail(string imagePath);
    Task Upload(VideoReqDto videoDto, int userId);
    Task Like(int videoId, int userId);
    Task Dislike(int videoId, int userId);
    Task UpdateThumbnail(int videoId, int userId);
    Task Update(int id, VideoReqDto videoDto, int userId);
    Task Delete(int id, int userId);
}