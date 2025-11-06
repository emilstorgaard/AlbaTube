using Microsoft.EntityFrameworkCore;
using AlbaTube.Models;

namespace AlbaTube.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<LikedVideo> LikedVideos { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure that when a User is deleted, their Videos are also deleted
        modelBuilder.Entity<Video>()
            .HasOne(s => s.User)
            .WithMany(u => u.Videos)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Deletes videos when user is deleted

        // Configure the many-to-many relationship between User and Video through LikedVideo
        modelBuilder.Entity<LikedVideo>()
            .HasKey(lv => new { lv.UserId, lv.VideoId });

        modelBuilder.Entity<LikedVideo>()
            .HasOne(ls => ls.User)
            .WithMany(u => u.LikedVideos)
            .HasForeignKey(ls => ls.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<LikedVideo>()
            .HasOne(ls => ls.Video)
            .WithMany(s => s.LikedVideos)
            .HasForeignKey(ls => ls.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the many-to-many self-referencing relationship between Users through Subscription
        modelBuilder.Entity<Subscription>()
            .HasKey(s => new { s.SubscriberId, s.CreatorId });

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Subscriber)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.SubscriberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Creator)
            .WithMany(u => u.Subscribers)
            .HasForeignKey(s => s.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Default value configuration for CreatedAt and UpdatedAt property
        modelBuilder.Entity<User>()
            .Property(s => s.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<User>()
            .Property(s => s.UpdatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Video>()
            .Property(s => s.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Video>()
            .Property(s => s.UpdatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<LikedVideo>()
            .Property(ls => ls.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
