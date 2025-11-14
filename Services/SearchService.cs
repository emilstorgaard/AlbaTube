using AlbaTube.Dtos.Response;
using AlbaTube.Exceptions;
using AlbaTube.Mappers;
using AlbaTube.Repositories.Interfaces;
using AlbaTube.Services.Interfaces;

namespace AlbaTube.Services;

public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly IUserRepository _userRepository;

    public SearchService(ISearchRepository searchRepository, IVideoRepository videoRepository, IUserRepository userRepository)
    {
        _searchRepository = searchRepository;
        _videoRepository = videoRepository;
        _userRepository = userRepository;
    }

    public async Task<List<VideoRespDto>> Videos(string query, int loggedInUserId)
    {
        var videos = await _searchRepository.Videos(query);

        var videoDtos = new List<VideoRespDto>();

        foreach (var video in videos)
        {
            var likeCount = await _videoRepository.GetLikeCount(video.Id);
            var isLiked = await _videoRepository.IsVideoLikedByUser(video.Id, loggedInUserId);
            var dto = VideoMapper.MapToDto(video, likeCount, isLiked);
            videoDtos.Add(dto);
        }

        return videoDtos;
    }

    public async Task<List<UserRespDto>> Users(string query, int loggedInUserId)
    {
        var users = await _searchRepository.Users(query);

        var userDtos = new List<UserRespDto>();

        foreach (var creator in users)
        {
            var subscriberCount = await _userRepository.GetSubscriberCount(creator.Id);
            var isSubscribed = await _userRepository.IsSubscribedAsync(loggedInUserId, creator.Id);
            var dto = UserMapper.MapToDto(creator, subscriberCount, isSubscribed);
            userDtos.Add(dto);
        }

        return userDtos;
    }
}
