using AlbaTube.Dtos.Request;
using AlbaTube.Dtos.Response;

namespace AlbaTube.Services.Interfaces;

public interface IVideoService
{
    Task<FileStream> Stream(int id);
    Task<VideoRespDto> GetVideoById(int id);
    Task<List<VideoRespDto>> GetAllVideosByUserId(int userId);
    string GetThumbnail(string imagePath);
    Task Upload(VideoReqDto videoDto, int userId);
    Task Like(int videoId, int userId);
    Task Dislike(int videoId, int userId);
    Task UpdateThumbnail(int videoId, int userId);
    Task Update(int id, VideoReqDto videoDto, int userId);
    Task Delete(int id, int userId);
}