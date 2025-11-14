using AlbaTube.Dtos.Response;

namespace AlbaTube.Services.Interfaces;

public interface ISearchService
{
    Task<List<VideoRespDto>> Videos(string query, int loggedInUserId);
    Task<List<UserRespDto>> Users(string query, int loggedInUserId);
}
