namespace AlbaTube.Models;

public class Subscription
{
    public int SubscriberId { get; set; }
    public int CreatorId { get; set; }

    public User Subscriber { get; set; } = null!;
    public User Creator { get; set; } = null!;
}
