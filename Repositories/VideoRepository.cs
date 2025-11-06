using Microsoft.EntityFrameworkCore;
using AlbaTube.Database;
using AlbaTube.Models;
using AlbaTube.Repositories.Interfaces;

namespace AlbaTube.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VideoRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Video?> GetVideoById(int id)
    {
        return await _dbContext.Videos.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task IncrementViewCount(int videoId)
    {
        var video = await _dbContext.Videos.FindAsync(videoId);
        if (video == null) return;

        video.ViewCount += 1;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Video>> GetVideosByUserId(int userId)
    {
        return await _dbContext.Videos
            .AsNoTracking()
            .Where(v => v.UserId == userId)
            .ToListAsync();
    }

    public async Task<Video?> GetExsistingVideo(string title)
    {
        return await _dbContext.Videos.AsNoTracking().FirstOrDefaultAsync(v => v.Title == title);
    }

    public async Task AddVideo(Video video)
    {
        await _dbContext.Videos.AddAsync(video);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<LikedVideo?> GetLikedVideoByUser(int videoId, int userId)
    {
        return await _dbContext.LikedVideos.FirstOrDefaultAsync(lv => lv.VideoId == videoId && lv.UserId == userId);
    }

    public async Task<List<int>> GetLikedVideoIdsByUser(int userId)
    {
        return await _dbContext.LikedVideos
            .AsNoTracking()
            .Where(lv => lv.UserId == userId)
            .Include(lv => lv.Video)
            .Select(lv => lv.Video.Id)
            .ToListAsync();
    }

    public async Task<bool> IsVideoLikedByUser(int videoId, int userId)
    {
        return await _dbContext.LikedVideos
            .AsNoTracking()
            .AnyAsync(lv => lv.VideoId == videoId && lv.UserId == userId);
    }

    public async Task LikeVideo(LikedVideo likedVideo)
    {
        await _dbContext.LikedVideos.AddAsync(likedVideo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DislikeVideo(LikedVideo likedVideo)
    {
        _dbContext.LikedVideos.Remove(likedVideo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateVideo(Video video)
    {
        _dbContext.Videos.Update(video);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteVideo(Video video)
    {
        _dbContext.Videos.Remove(video);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteLikedVideos(int userId)
    {
        var likedVideos = _dbContext.LikedVideos.Where(lv => lv.UserId == userId);
        _dbContext.LikedVideos.RemoveRange(likedVideos);
        await _dbContext.SaveChangesAsync();
    }
}
