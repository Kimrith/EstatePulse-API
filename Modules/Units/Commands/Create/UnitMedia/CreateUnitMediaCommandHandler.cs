using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using EstatePulse.Modules.Units.Models;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Create;

public sealed class CreateUnitMediaCommandHandler : IRequestHandler<CreateUnitMediaCommand, UnitMediaDto>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public CreateUnitMediaCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<UnitMediaDto> Handle(CreateUnitMediaCommand request, CancellationToken cancellationToken)
    {
        if (!request.UnitId.HasValue && !request.UnitTypeId.HasValue)
        {
            throw new ArgumentException("Media must be associated with either a UnitId or a UnitTypeId.");
        }

        string? unitCode = null;
        if (request.UnitId.HasValue)
        {
            var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == request.UnitId.Value, cancellationToken);
            if (unit == null)
            {
                throw new KeyNotFoundException($"Unit with ID {request.UnitId.Value} was not found.");
            }
            unitCode = unit.UnitCode;
        }

        string? unitTypeCode = null;
        if (request.UnitTypeId.HasValue)
        {
            var unitType = await _context.UnitTypes.FirstOrDefaultAsync(ut => ut.Id == request.UnitTypeId.Value, cancellationToken);
            if (unitType == null)
            {
                throw new KeyNotFoundException($"Unit type with ID {request.UnitTypeId.Value} was not found.");
            }
            unitTypeCode = unitType.Code;
        }

        string finalMediaUrl = request.MediaUrl ?? string.Empty;

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

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.MediaFile.CopyToAsync(stream, cancellationToken);
            }

            finalMediaUrl = $"/uploads/units/{uniqueFileName}";
        }

        if (string.IsNullOrWhiteSpace(finalMediaUrl))
        {
            throw new ArgumentException("Either MediaFile or MediaUrl must be provided.");
        }

        var media = new UnitMedia
        {
            UnitId = request.UnitId,
            UnitTypeId = request.UnitTypeId,
            MediaType = request.MediaType,
            MediaUrl = finalMediaUrl,
            SortOrder = request.SortOrder,
            IsFeatured = request.IsFeatured
        };

        _context.UnitMedia.Add(media);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-media");

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
