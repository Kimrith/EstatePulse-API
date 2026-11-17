using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

[Table("UserFollows")]
public class UserFollows
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("following_user_id")]
    public int FollowingUserId { get; set; }

    [Required]
    [Column("followed_user_id")]
    public int FollowedUserId { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey(nameof(FollowingUserId))]
    [InverseProperty(nameof(User.Followings))]
    public virtual User FollowingUser { get; set; } = null!;

    [ForeignKey(nameof(FollowedUserId))]
    [InverseProperty(nameof(User.Followers))]
    public virtual User FollowedUser { get; set; } = null!;
}
