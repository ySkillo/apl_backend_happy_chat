namespace HappyChat.Domain.Users.Entities;

public class Follow
{
    public long FollowerId { get; set; }   // quem segue
    public long FollowedId { get; set; }   // quem é seguido
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Follower { get; set; } = null!;
    public User Followed { get; set; } = null!;
}