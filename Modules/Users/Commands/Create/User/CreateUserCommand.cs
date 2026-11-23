using EstatePulse.Common.Enums;
using EstatePulse.Modules.Users.DTOs;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Users.Commands.Create;

public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    UserRole Role,
    string? NationalIdCard,
    IFormFile? Avatar
) : IRequest<UserDto>;
