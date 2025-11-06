namespace AlbaTube;

public class Settings
{
    public string VideoFolder { get; set; }
    public string ImageFolder { get; set; }
    public string[] AllowedVideoExtensions { get; set; }
    public string[] AllowedImageExtensions { get; set; }
    public string JwtSecret { get; set; }
    public int JwtExpiryHours { get; set; }
}
