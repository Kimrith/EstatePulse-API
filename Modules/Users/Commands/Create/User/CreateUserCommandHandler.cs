using System.IO;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Users.DTOs;
using EstatePulse.Modules.Users.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Commands.Create;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public CreateUserCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");
        }

        string? avatarUrl = null;
        if (request.Avatar != null && request.Avatar.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Avatar.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Avatar.CopyToAsync(stream, cancellationToken);
            }

            avatarUrl = $"/uploads/avatars/{fileName}";
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = passwordHash,
            Role = request.Role,
            NationalIdCard = request.NationalIdCard,
            AvatarUrl = avatarUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("users");

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            NationalIdCard = user.NationalIdCard,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
