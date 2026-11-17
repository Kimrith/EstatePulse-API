using System.ComponentModel.DataAnnotations;

namespace EstatePulse.DTOs
{
    public class UserFollowsDto
    {
        public int Id { get; set; }
        public int FollowingUserId { get; set; }
        public int FollowedUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Optional lightweight user profiles to avoid returning full User entities
        public UserSummaryDto? FollowingUser { get; set; }
        public UserSummaryDto? FollowedUser { get; set; }
    }

    public class FollowUserRequestDto
    {
        [Required]
        public int FollowedUserId { get; set; }
    }

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}