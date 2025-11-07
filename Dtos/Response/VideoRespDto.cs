namespace AlbaTube.Dtos.Response;

public class VideoRespDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required TimeSpan Duration { get; set; }
    public required string VideoPath { get; set; }
    public required string ThumbnailPath { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public bool IsLiked { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
