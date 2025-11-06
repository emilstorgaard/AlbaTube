using Microsoft.IdentityModel.Tokens;
using AlbaTube.Dtos.Response;
using AlbaTube.Exceptions;
using AlbaTube.Helpers;
using AlbaTube.Repositories.Interfaces;
using AlbaTube.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AlbaTube.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly Settings _settings;

    public AuthService(Settings settings, IUserRepository userRepository)
    {
        _settings = settings;
        _userRepository = userRepository;
    }

    public async Task<TokenRespDto> Login(string email, string password)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash)) throw new UnauthorizedException("Invalid email or password.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim("uid", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        }),
            Expires = DateTime.UtcNow.AddHours(_settings.JwtExpiryHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var tokenRespDto = new TokenRespDto
        {
            Token = tokenString
        };

        return tokenRespDto;
    }
}
