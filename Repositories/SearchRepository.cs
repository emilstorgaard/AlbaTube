using AlbaTube.Database;
using AlbaTube.Models;
using AlbaTube.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlbaTube.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SearchRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Video>> Videos(string query)
    {
        return await _dbContext.Videos
            .Where(v => v.Title.Contains(query) || v.Description.Contains(query))
            .ToListAsync();
    }

    public async Task<List<User>> Users(string query)
    {
        return await _dbContext.Users
            .Where(u => u.Username.Contains(query) || u.Email.Contains(query))
            .ToListAsync();
    }
}
