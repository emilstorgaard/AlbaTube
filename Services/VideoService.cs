using AlbaTube.Dtos.Request;
using AlbaTube.Dtos.Response;
using AlbaTube.Exceptions;
using AlbaTube.Helpers;
using AlbaTube.Mappers;
using AlbaTube.Models;
using AlbaTube.Repositories.Interfaces;
using AlbaTube.Services.Interfaces;

namespace AlbaTube.Services;

public class VideoService : IVideoService
{
    private readonly Settings _settings;
    private readonly IVideoRepository _videoRepository;
    private readonly IUserRepository _userRepository;

    public VideoService(Settings settings, IVideoRepository videoRepository, IUserRepository userRepository)
    {
        _settings = settings;
        Directory.CreateDirectory(_settings.VideoFolder);
        Directory.CreateDirectory(_settings.ImageFolder);
        _videoRepository = videoRepository;
        _userRepository = userRepository;
    }

    public async Task<FileStream> Stream(int id)
    {
        var video = await _videoRepository.GetVideoById(id);
        if (video == null) throw new NotFoundException("Video not found.");

        if (string.IsNullOrEmpty(video?.VideoPath)) throw new NotFoundException("Video file path is missing.");

        var videoPath = FileHelper.GetFullPath(video.VideoPath);
        if (!File.Exists(videoPath)) throw new NotFoundException("Video file not found.");

        var fileStream = File.OpenRead(videoPath);
        return fileStream;
    }

    public async Task<VideoRespDto> GetVideoById(int id, int loggedInUserId)
    {
        var video = await _videoRepository.GetVideoById(id);
        if (video == null) throw new NotFoundException("Video not found.");

        var likeCount = await _videoRepository.GetLikeCount(id);
        var isLiked = await _videoRepository.IsVideoLikedByUser(id, loggedInUserId);

        var videoDto = VideoMapper.MapToDto(video, likeCount, isLiked);
        return videoDto;
    }

    public async Task<List<VideoRespDto>> GetPopularVideos(int loggedInUserId)
    {
        var videos = await _videoRepository.GetPopularVideos();
        if (!videos.Any()) throw new NotFoundException("No videos found.");

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

    public async Task<List<VideoRespDto>> GetAllVideosByUserId(int userId, int loggedInUserId)
    {
        var videos = await _videoRepository.GetVideosByUserId(userId);

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

    public string GetThumbnail(string imagePath)
    {
        var thumbnailPath = FileHelper.GetFullPath(imagePath);

        if (!System.IO.File.Exists(thumbnailPath)) throw new NotFoundException("Thumbnail not found.");

        return thumbnailPath;
    }

    public async Task Upload(VideoReqDto videoDto, int userId)
    {
        if (videoDto == null || videoDto.Video == null || !FileHelper.IsValidFile(videoDto.Video, _settings.AllowedVideoExtensions))
            throw new BadRequestException("Invalid video data.");

        var user = await _userRepository.GetUserById(userId);
        if (user == null) throw new NotFoundException("User not found.");

        var existingVideo = await _videoRepository.GetExsistingVideo(videoDto.Title);
        if (existingVideo != null) throw new ConflictException("A video with the same title already exists.");

        var videoPath = FileHelper.SaveFile(videoDto.Video, _settings.VideoFolder);
        var thumbnailPath = videoDto.Thumbnail != null && FileHelper.IsValidFile(videoDto.Thumbnail, _settings.AllowedImageExtensions)
            ? FileHelper.SaveFile(videoDto.Thumbnail, _settings.ImageFolder)
            : FileHelper.GetDefaultThumbnailPath(_settings.ImageFolder);

        var video = new Video
        {
            Title = videoDto.Title,
            Description = videoDto.Description,
            Duration = videoDto.Duration,
            VideoPath = videoPath,
            ThumbnailPath = thumbnailPath,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            User = user
        };

        await _videoRepository.AddVideo(video);
    }

    public async Task Like(int videoId, int userId)
    {   
        var isAlreadyLiked = await _videoRepository.IsVideoLikedByUser(videoId, userId);
        if (isAlreadyLiked) throw new ConflictException("Video already liked.");

        var likedVideo = new LikedVideo
        {
            UserId = userId,
            VideoId = videoId
        };

        var video = await _videoRepository.GetVideoById(videoId);
        if (video == null) throw new NotFoundException("Video not found.");

        video.UpdatedAtUtc = DateTime.UtcNow;

        await _videoRepository.LikeVideo(likedVideo);
    }

    public async Task Dislike(int videoId, int userId)
    {
        var likedVideo = await _videoRepository.GetLikedVideoByUser(videoId, userId);
        if (likedVideo == null) throw new NotFoundException("Video not found in your liked videos.");

        var video = await _videoRepository.GetVideoById(videoId);
        if (video == null) throw new NotFoundException("Video not found.");

        video.UpdatedAtUtc = DateTime.UtcNow;

        await _videoRepository.DislikeVideo(likedVideo);
    }

    public async Task UpdateThumbnail(int videoId, int userId)
    {
        var video = await _videoRepository.GetVideoById(videoId);
        if (video == null) throw new NotFoundException("Video not found.");
        if (video.UserId != userId) throw new UnauthorizedException("You are not allowed to update this video.");

        FileHelper.DeleteFile(video.ThumbnailPath);

        video.ThumbnailPath = FileHelper.GetDefaultThumbnailPath(_settings.VideoFolder);
        video.UpdatedAtUtc = DateTime.UtcNow;

        await _videoRepository.UpdateVideo(video);
    }

    public async Task Update(int id, VideoReqDto videoDto, int userId)
    {
        var video = await _videoRepository.GetVideoById(id);
        if (video == null) throw new NotFoundException("Video was not found.");
        if (video.UserId != userId) throw new UnauthorizedException("You are not allowed to update this video.");

        var existingVideo = await _videoRepository.GetExsistingVideo(videoDto.Title);
        if (existingVideo != null && existingVideo.Id != id) throw new NotFoundException("A video with the same title already exists.");

        if (videoDto.Thumbnail != null && FileHelper.IsValidFile(videoDto.Thumbnail, _settings.AllowedImageExtensions))
        {
            FileHelper.DeleteFile(video.ThumbnailPath);
            video.ThumbnailPath = FileHelper.SaveFile(videoDto.Thumbnail, _settings.ImageFolder);
        }

        if (videoDto.Video != null && FileHelper.IsValidFile(videoDto.Video, _settings.AllowedVideoExtensions))
        {
            FileHelper.DeleteFile(video.VideoPath);
            video.VideoPath = FileHelper.SaveFile(videoDto.Video, _settings.VideoFolder);
        }

        video.Title = videoDto.Title;
        video.Description = videoDto.Description;
        video.Duration = videoDto.Duration;
        video.UpdatedAtUtc = DateTime.UtcNow;

        await _videoRepository.UpdateVideo(video);
    }

    public async Task Delete(int id, int userId)
    {
        var video = await _videoRepository.GetVideoById(id);
        if (video == null) throw new NotFoundException("Video not found.");
        if (video.UserId != userId) throw new UnauthorizedException("You are not allowed to delete this video.");

        FileHelper.DeleteFile(video.VideoPath);
        FileHelper.DeleteFile(video.ThumbnailPath);

        await _videoRepository.DeleteVideo(video);
    }
}
