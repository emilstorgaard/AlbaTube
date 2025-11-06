using AlbaTube.Dtos.Request;
using AlbaTube.Dtos.Response;

namespace AlbaTube.Services.Interfaces;

public interface IUserService
{
    Task<List<UserRespDto>> GetAll();
    Task<UserRespDto> GetUser(int userId);
    Task AddUser(UserReqDto userReqDto);
    Task Delete(int userId);
    Task Subscribe(int subscriberId, int creatorId);
    Task Unsubscribe(int subscriberId, int creatorId);
}