using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Users.DTOs;

public sealed class FollowUserRequestDto
{
    [Required]
    public int FollowedUserId { get; set; }
}
