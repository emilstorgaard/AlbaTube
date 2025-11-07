namespace AlbaTube.Dtos.Response;

public class UserRespDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string ProfileImageParh { get; set; }
    public required int SubscriberCount { get; set; }
    public bool IsSubscribed { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
