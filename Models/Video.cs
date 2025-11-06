using System.Text.Json.Serialization;

namespace AlbaTube.Models;

public class Video
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required TimeSpan Duration { get; set; }
    public required string VideoPath { get; set; }
    public required string ThumbnailPath { get; set; }
    public int UserId { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public required User User { get; set; }

    [JsonIgnore]
    public ICollection<LikedVideo> LikedVideos { get; set; } = new List<LikedVideo>();
}
