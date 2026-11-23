using System.ComponentModel.DataAnnotations;
using EstatePulse.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Users.DTOs;

public sealed class UserUpdateDto
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
