namespace AlbaTube.Dtos.Request;

public class VideoReqDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required TimeSpan Duration { get; set; }
    public IFormFile? Video { get; set; }
    public IFormFile? Thumbnail { get; set; }
}
