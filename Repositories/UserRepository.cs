using Microsoft.EntityFrameworkCore;
using AlbaTube.Database;
using AlbaTube.Models;
using AlbaTube.Repositories.Interfaces;

namespace AlbaTube.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<List<User>> GetSubscriptionsById(int loggedInUserId)
    {
        return await _dbContext.Subscriptions
            .Where(s => s.SubscriberId == loggedInUserId)
            .Select(s => s.Creator)
            .ToListAsync();
    }

    public async Task AddUser(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(User user)
    {
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Subscribe(Subscription subscription)
    {
        await _dbContext.Subscriptions.AddAsync(subscription);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UnSubscribe(Subscription subscription)
    {
        _dbContext.Subscriptions.Remove(subscription);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsSubscribedAsync(int loggedInUserId, int creatorId)
    {
        return await _dbContext.Subscriptions.AnyAsync(s => s.SubscriberId == loggedInUserId && s.CreatorId == creatorId);
    }

    public async Task<Subscription?> GetSubscription(int subscriberId, int creatorId)
    {
        return await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.SubscriberId == subscriberId && s.CreatorId == creatorId);
    }

    public async Task<int> GetSubscriberCount(int userId)
    {
        return await _dbContext.Subscriptions.CountAsync(u => u.CreatorId == userId);
    }
}
