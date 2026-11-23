using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed class UpdateUnitTypeCommandHandler : IRequestHandler<UpdateUnitTypeCommand, UnitTypeDto?>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public UpdateUnitTypeCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<UnitTypeDto?> Handle(UpdateUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.UnitTypes
            .FirstOrDefaultAsync(ut => ut.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.Code) && !string.Equals(request.Code, entity.Code, StringComparison.OrdinalIgnoreCase))
        {
            var codeExists = await _context.UnitTypes
                .AnyAsync(ut => ut.Code.ToLower() == request.Code.ToLower() && ut.Id != request.Id, cancellationToken);

            if (codeExists)
            {
                throw new InvalidOperationException($"A Unit Type with code '{request.Code}' already exists.");
            }
            entity.Code = request.Code;
        }

        if (request.FloorPlanImageFile != null && request.FloorPlanImageFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
            var extension = Path.GetExtension(request.FloorPlanImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid floor plan image type. Only JPG, PNG, WEBP, and SVG are allowed.");
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "unit-types");
            Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrEmpty(entity.FloorPlanImageUrl))
            {
                var relativePath = entity.FloorPlanImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
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
                await request.FloorPlanImageFile.CopyToAsync(stream, cancellationToken);
            }

            entity.FloorPlanImageUrl = $"/uploads/unit-types/{uniqueFileName}";
        }

        if (!string.IsNullOrWhiteSpace(request.TypeName)) entity.TypeName = request.TypeName;
        if (request.Bedrooms.HasValue) entity.Bedrooms = request.Bedrooms.Value;
        if (request.Bathrooms.HasValue) entity.Bathrooms = request.Bathrooms.Value;
        if (request.ParkingSlots.HasValue) entity.ParkingSlots = request.ParkingSlots.Value;
        if (request.Floors.HasValue) entity.Floors = request.Floors.Value;
        if (request.BuildingSize.HasValue) entity.BuildingSize = request.BuildingSize.Value;
        if (request.LandSizeRecommended.HasValue) entity.LandSizeRecommended = request.LandSizeRecommended.Value;
        if (request.SpecificationJson != null) entity.SpecificationJson = request.SpecificationJson;

        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-types");

        var totalUnitsCount = await _context.Units
            .CountAsync(u => u.UnitTypeId == entity.Id, cancellationToken);

        return new UnitTypeDto
        {
            Id = entity.Id,
            TypeName = entity.TypeName,
            Code = entity.Code,
            Bedrooms = entity.Bedrooms,
            Bathrooms = entity.Bathrooms,
            ParkingSlots = entity.ParkingSlots,
            Floors = entity.Floors,
            BuildingSize = entity.BuildingSize,
            LandSizeRecommended = entity.LandSizeRecommended,
            FloorPlanImageUrl = entity.FloorPlanImageUrl,
            SpecificationJson = entity.SpecificationJson,
            TotalUnitsCount = totalUnitsCount
        };
    }
}
