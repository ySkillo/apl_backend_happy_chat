using HappyChat.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace HappyChat.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<PendingRegistration> PendingRegistrations => Set<PendingRegistration>();

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
    }
}