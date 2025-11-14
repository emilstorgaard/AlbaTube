using AlbaTube.Models;

namespace AlbaTube.Repositories.Interfaces;

public interface ISearchRepository
{
    Task<List<Video>> Videos(string query);
    Task<List<User>> Users(string query);
}
