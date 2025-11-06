using System.Text.Json.Serialization;

namespace AlbaTube.Models;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string ProfileImagePath { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    [JsonIgnore]
    public ICollection<Video> Videos { get; set; } = new List<Video>();

    [JsonIgnore]
    public ICollection<LikedVideo> LikedVideos { get; set; } = new List<LikedVideo>();

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public ICollection<Subscription> Subscribers { get; set; } = new List<Subscription>();
}
