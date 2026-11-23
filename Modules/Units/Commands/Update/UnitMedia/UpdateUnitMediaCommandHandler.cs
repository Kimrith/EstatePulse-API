using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed class UpdateUnitMediaCommandHandler : IRequestHandler<UpdateUnitMediaCommand, UnitMediaDto?>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public UpdateUnitMediaCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<UnitMediaDto?> Handle(UpdateUnitMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await _context.UnitMedia
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (media == null)
        {
            return null;
        }

        if (request.UnitId.HasValue && request.UnitId.Value != media.UnitId)
        {
            var unitExists = await _context.Units.AnyAsync(u => u.Id == request.UnitId.Value, cancellationToken);
            if (!unitExists)
            {
                throw new KeyNotFoundException($"Unit with ID {request.UnitId.Value} was not found.");
            }
            media.UnitId = request.UnitId.Value;
        }

        if (request.UnitTypeId.HasValue && request.UnitTypeId.Value != media.UnitTypeId)
        {
            var unitTypeExists = await _context.UnitTypes.AnyAsync(ut => ut.Id == request.UnitTypeId.Value, cancellationToken);
            if (!unitTypeExists)
            {
                throw new KeyNotFoundException($"Unit type with ID {request.UnitTypeId.Value} was not found.");
            }
            media.UnitTypeId = request.UnitTypeId.Value;
        }

        if (request.MediaFile != null && request.MediaFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".mp4", ".svg", ".pdf" };
            var extension = Path.GetExtension(request.MediaFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid media file type.");
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "units");
            Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrEmpty(media.MediaUrl) && media.MediaUrl.StartsWith("/uploads/units/"))
            {
                var relativePath = media.MediaUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var oldFilePath = Path.Combine(webRoot, relativePath);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.MediaFile.CopyToAsync(stream, cancellationToken);
            }

            media.MediaUrl = $"/uploads/units/{uniqueFileName}";
        }
        else if (!string.IsNullOrWhiteSpace(request.MediaUrl))
        {
            media.MediaUrl = request.MediaUrl;
        }

        if (!string.IsNullOrWhiteSpace(request.MediaType)) media.MediaType = request.MediaType;
        if (request.SortOrder.HasValue) media.SortOrder = request.SortOrder.Value;
        if (request.IsFeatured.HasValue) media.IsFeatured = request.IsFeatured.Value;

        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-media");

        string? unitCode = null;
        if (media.UnitId.HasValue)
        {
            unitCode = await _context.Units
                .Where(u => u.Id == media.UnitId.Value)
                .Select(u => u.UnitCode)
                .FirstOrDefaultAsync(cancellationToken);
        }

        string? unitTypeCode = null;
        if (media.UnitTypeId.HasValue)
        {
            unitTypeCode = await _context.UnitTypes
                .Where(ut => ut.Id == media.UnitTypeId.Value)
                .Select(ut => ut.Code)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new UnitMediaDto
        {
            Id = media.Id,
            UnitId = media.UnitId,
            UnitCode = unitCode,
            UnitTypeId = media.UnitTypeId,
            UnitTypeCode = unitTypeCode,
            MediaType = media.MediaType,
            MediaUrl = media.MediaUrl,
            SortOrder = media.SortOrder,
            IsFeatured = media.IsFeatured
        };
    }
}
