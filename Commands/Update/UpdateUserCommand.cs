using EstatePulse.Data;
using EstatePulse.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Update
{
    public class UpdateUserCommand : IRequest<UserDto?>
    {
        public UserUpdateDto User { get; set; } = new UserUpdateDto();
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto?>
    {
        private readonly AppDbContext _context;

        public UpdateUserCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<UserDto?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.User;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == dto.Id, cancellationToken);

            if (user == null)
            {
                return null;
            }

            var emailTaken = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != dto.Id, cancellationToken);

            if (emailTaken)
            {
                throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Role = dto.Role;
            user.NationalIdCard = dto.NationalIdCard;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            if (dto.Avatar != null && dto.Avatar.Length > 0)
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
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Avatar.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Avatar.CopyToAsync(stream, cancellationToken);
                }

                user.AvatarUrl = $"/uploads/avatars/{fileName}";
            }

            await _context.SaveChangesAsync(cancellationToken);

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
}