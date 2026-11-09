using EstatePulse.Data;
using EstatePulse.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Queries
{
    public static class UserQueries
    {
        public static async Task<PaginatedList<UserDto>> GetUsersAsync(
            AppDbContext context,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 12;

            int offset = (pageNumber - 1) * pageSize;

            IQueryable<Models.User> query;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Removed ORDER BY from raw SQL so EF Core can wrap it safely in subqueries (Count, Skip/Take)
                query = context.Users.FromSqlInterpolated($@"
                    SELECT * 
                    FROM [Users]
                ");
            }
            else
            {
                var term = $"%{searchTerm.Trim()}%";

                query = context.Users.FromSqlInterpolated($@"
                    SELECT * 
                    FROM [Users]
                    WHERE [FullName] LIKE {term} 
                       OR [Email] LIKE {term}
                ");
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt) // Applied cleanly at the outer query level
                .Skip(offset)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role,
                    NationalIdCard = u.NationalIdCard,
                    AvatarUrl = u.AvatarUrl,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return new PaginatedList<UserDto>(items, totalCount, pageNumber, pageSize);
        }

        public static async Task<UserDto?> GetUserByIdAsync(
            AppDbContext context,
            int id,
            CancellationToken cancellationToken = default)
        {
            return await context.Users
                .FromSqlInterpolated($@"
                    SELECT TOP 1 * 
                    FROM [Users] 
                    WHERE [Id] = {id}
                ")
                .AsNoTracking()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role,
                    NationalIdCard = u.NationalIdCard,
                    AvatarUrl = u.AvatarUrl,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public static async Task<UserDto?> GetUserByNameAsync(
            AppDbContext context,
            string fullName,
            CancellationToken cancellationToken = default)
        {
            var trimmedName = fullName.Trim();

            return await context.Users
                .FromSqlInterpolated($@"
                    SELECT TOP 1 * 
                    FROM [Users] 
                    WHERE LOWER([FullName]) = LOWER({trimmedName})
                ")
                .AsNoTracking()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role,
                    NationalIdCard = u.NationalIdCard,
                    AvatarUrl = u.AvatarUrl,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public static async Task<UserDto?> GetUserByEmailAsync(
            AppDbContext context,
            string email,
            CancellationToken cancellationToken = default)
        {
            var trimmedEmail = email.Trim();

            return await context.Users
                .FromSqlInterpolated($@"
                    SELECT TOP 1 * 
                    FROM [Users] 
                    WHERE LOWER([Email]) = LOWER({trimmedEmail})
                ")
                .AsNoTracking()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role,
                    NationalIdCard = u.NationalIdCard,
                    AvatarUrl = u.AvatarUrl,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}