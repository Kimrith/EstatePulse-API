using EstatePulse.Common.Enums;
using EstatePulse.Modules.Users.DTOs;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Users.Commands.Update;

public sealed record UpdateUserCommand(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string? Password,
    UserRole Role,
    string? NationalIdCard,
    IFormFile? Avatar,
    bool IsActive
) : IRequest<UserDto?>;
