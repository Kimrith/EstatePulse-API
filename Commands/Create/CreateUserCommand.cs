using System.IO;
using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Create
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public UserCreateDto User { get; set; } = new UserCreateDto();
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly AppDbContext _context;

        public CreateUserCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.User;

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower(), cancellationToken);

            if (emailExists)
            {
                throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");
            }

            // Handle file upload if an avatar is provided
            string? avatarUrl = null;
            if (dto.Avatar != null && dto.Avatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Avatar.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Avatar.CopyToAsync(stream, cancellationToken);
                }

                avatarUrl = $"/uploads/avatars/{fileName}";
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                Role = dto.Role,
                NationalIdCard = dto.NationalIdCard,
                AvatarUrl = avatarUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
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