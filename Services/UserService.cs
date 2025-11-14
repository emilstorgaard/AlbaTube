using AlbaTube.Dtos.Request;
using AlbaTube.Dtos.Response;
using AlbaTube.Exceptions;
using AlbaTube.Helpers;
using AlbaTube.Mappers;
using AlbaTube.Models;
using AlbaTube.Repositories.Interfaces;
using AlbaTube.Services.Interfaces;

namespace AlbaTube.Services;

public class UserService : IUserService
{
    private readonly Settings _settings;
    private readonly IUserRepository _userRepository;
    private readonly IVideoRepository _videoRepository;

    public UserService(Settings settings, IUserRepository userRepository, IVideoRepository videoRepository)
    {
        _settings = settings;
        Directory.CreateDirectory(_settings.ImageFolder);
        _userRepository = userRepository;
        _videoRepository = videoRepository;
    }

    public async Task<List<UserRespDto>> GetAll(int loggedInUserId)
    {
        var users = await _userRepository.GetAllUsers();
        if (!users.Any()) throw new NotFoundException("No users found.");

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

    public async Task<UserRespDto> GetUser(int userId, int loggedInUserId)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user == null) throw new NotFoundException("User not found.");

        var subscriberCount = await _userRepository.GetSubscriberCount(userId);

        var isSubscribed = await _userRepository.IsSubscribedAsync(loggedInUserId, userId);

        var userDto = UserMapper.MapToDto(user, subscriberCount, isSubscribed);
        return userDto;
    }

    public async Task<List<UserRespDto>> GetSubscriptionsById(int loggedInUserId)
    {
        var users = await _userRepository.GetSubscriptionsById(loggedInUserId);
        if (users == null || !users.Any())
            throw new NotFoundException("No subscriptions found");

        var userDtos = new List<UserRespDto>();

        foreach (var user in users)
        {
            var subscriberCount = await _userRepository.GetSubscriberCount(user.Id);
            var isSubscribed = await _userRepository.IsSubscribedAsync(loggedInUserId, user.Id);

            var dto = UserMapper.MapToDto(user, subscriberCount, isSubscribed);
            userDtos.Add(dto);
        }

        return userDtos;
    }

    public async Task AddUser(UserReqDto userReqDto)
    {
        var existingEmail = await _userRepository.GetUserByEmail(userReqDto.Email);
        if (existingEmail != null) throw new ConflictException("User with this email already exists.");

        var existingUsername = await _userRepository.GetUserByUsername(userReqDto.Username);
        if (existingUsername != null) throw new ConflictException("User with this name already exists.");

        var passwordHash = PasswordHelper.HashPassword(userReqDto.Password);

        var profileImagePath = userReqDto.ProfileImage != null && FileHelper.IsValidFile(userReqDto.ProfileImage, _settings.AllowedImageExtensions) ? FileHelper.SaveFile(userReqDto.ProfileImage, _settings.ImageFolder) : FileHelper.GetDefaultProfileImagePath(_settings.ImageFolder);

        var user = new User
        {
            Email = userReqDto.Email,
            Username = userReqDto.Username,
            PasswordHash = passwordHash,
            ProfileImagePath = profileImagePath,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _userRepository.AddUser(user);
    }

    public async Task Delete(int userId)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user == null) throw new NotFoundException("User not found.");

        await _videoRepository.DeleteLikedVideos(userId);
        await _userRepository.Delete(user);
    }

    public async Task Subscribe(int subscriberId, int creatorId)
    {
        if (subscriberId == creatorId)
            throw new InvalidOperationException("You cannot subscribe to yourself.");

        bool alreadySubscribed = await _userRepository.IsSubscribedAsync(subscriberId, creatorId);
        if (alreadySubscribed) throw new ConflictException("Video already liked.");

        var subscription = new Subscription
        {
            SubscriberId = subscriberId,
            CreatorId = creatorId
        };

        await _userRepository.Subscribe(subscription);
    }

    public async Task Unsubscribe(int subscriberId, int creatorId)
    {
        var subscription = await _userRepository.GetSubscription(subscriberId, creatorId);
        if (subscription == null) throw new NotFoundException("Subscription not found.");

        await _userRepository.UnSubscribe(subscription);
    }
}
