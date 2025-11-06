using AlbaTube.Models;

namespace AlbaTube.Repositories.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByUsername(string username);
    Task<User?> GetUserById(int id);
    Task AddUser(User user);
    Task Delete(User user);
    Task Subscribe(Subscription subscription);
    Task UnSubscribe(Subscription subscription);
    Task<bool> IsSubscribedAsync(int subscriberId, int creatorId);
    Task<Subscription?> GetSubscription(int subscriberId, int creatorId);
}