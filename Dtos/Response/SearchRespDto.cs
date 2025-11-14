namespace AlbaTube.Dtos.Response;

public class SearchRespDto
{
    public List<VideoRespDto> Videos { get; set; }
    public List<UserRespDto> Users { get; set; }
}
