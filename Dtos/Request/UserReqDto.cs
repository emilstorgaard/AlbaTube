namespace AlbaTube.Dtos.Request;

public class UserReqDto
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public IFormFile? ProfileImage { get; set; }
}
