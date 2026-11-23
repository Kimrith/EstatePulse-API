using System.ComponentModel.DataAnnotations;
using EstatePulse.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Users.DTOs;

public sealed class UserCreateDto
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
