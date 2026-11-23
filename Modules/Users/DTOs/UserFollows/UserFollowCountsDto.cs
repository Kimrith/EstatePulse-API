namespace EstatePulse.Modules.Users.DTOs;

public sealed class UserFollowCountsDto
{
    public int UserId { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
}
