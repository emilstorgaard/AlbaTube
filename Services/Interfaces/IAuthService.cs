using AlbaTube.Dtos.Response;

namespace AlbaTube.Services.Interfaces;

public interface IAuthService
{
    Task<TokenRespDto> Login(string email, string password);
}