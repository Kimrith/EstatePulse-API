namespace EstatePulse.Modules.Users.DTOs;

public sealed class UserFollowsDto
{
    public int Id { get; set; }
    public int FollowingUserId { get; set; }
    public int FollowedUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Optional lightweight user profiles to avoid returning full User entities
    public UserSummaryDto? FollowingUser { get; set; }
    public UserSummaryDto? FollowedUser { get; set; }
}
