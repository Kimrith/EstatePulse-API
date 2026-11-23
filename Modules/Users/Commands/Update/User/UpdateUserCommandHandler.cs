using System.IO;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Users.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Commands.Update;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public UpdateUserCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<UserDto?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
        {
            return null;
        }

        var emailTaken = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != request.Id, cancellationToken);

        if (emailTaken)
        {
            throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");
        }

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.Role = request.Role;
        user.NationalIdCard = request.NationalIdCard;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        if (request.Avatar != null && request.Avatar.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            // Delete old avatar if present
            if (!string.IsNullOrEmpty(user.AvatarUrl) && user.AvatarUrl.StartsWith("/uploads/avatars/"))
            {
                var relativePath = user.AvatarUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // Save new avatar
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Avatar.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Avatar.CopyToAsync(stream, cancellationToken);
            }

            user.AvatarUrl = $"/uploads/avatars/{fileName}";
        }

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
