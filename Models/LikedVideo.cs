namespace AlbaTube.Models;

public class LikedVideo
{
    public int UserId { get; set; }
    public int VideoId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Video Video { get; set; } = null!;
    public User User { get; set; } = null!;
}
