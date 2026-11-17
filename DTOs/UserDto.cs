using System.ComponentModel.DataAnnotations;
using EstatePulse.Enums;

namespace EstatePulse.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? NationalIdCard { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserCreateDto
    {
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Customer;

        [MaxLength(50)]
        public string? NationalIdCard { get; set; }

        public IFormFile? Avatar { get; set; }
    }

    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MinLength(6)]
        [MaxLength(100)]
        public string? Password { get; set; }

        public UserRole Role { get; set; }

        [MaxLength(50)]
        public string? NationalIdCard { get; set; }

        public IFormFile? Avatar { get; set; }

        public bool IsActive { get; set; }
    }
}