using HappyChat.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace HappyChat.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<PendingRegistration> PendingRegistrations => Set<PendingRegistration>();
    public DbSet<Follow> Follows => Set<Follow>();
    
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.ToTable("users");
            e.Property(u => u.Id).UseIdentityAlwaysColumn();
            e.Property(u => u.Name).HasMaxLength(100).IsRequired();
            e.Property(u => u.Perfil).HasMaxLength(50).IsRequired();
            e.Property(u => u.Email).HasMaxLength(254).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.HasIndex(u => u.Email).IsUnique().HasDatabaseName("ix_users_email");
            e.HasIndex(u => u.Perfil).IsUnique().HasDatabaseName("ix_users_perfil");
        });

        b.Entity<PendingRegistration>(e =>
        {
            e.ToTable("pending_registrations");
            e.HasKey(p => p.Email);
            e.Property(p => p.Email).HasMaxLength(254);
            e.HasIndex(p => p.ExpiresAt);
        });

        b.Entity<Follow>(e =>
        {
            e.ToTable("follows", t =>
                t.HasCheckConstraint("ck_follows_no_self_follow", "\"FollowerId\" <> \"FollowedId\""));

            e.HasKey(f => new { f.FollowerId, f.FollowedId });

            e.HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(f => f.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(f => f.FollowedId).HasDatabaseName("ix_follows_followed_id");
        });
    }
}