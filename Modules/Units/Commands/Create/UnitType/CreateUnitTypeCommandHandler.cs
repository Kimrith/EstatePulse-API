using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using EstatePulse.Modules.Units.Models;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Create;

public sealed class CreateUnitTypeCommandHandler : IRequestHandler<CreateUnitTypeCommand, UnitTypeDto>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public CreateUnitTypeCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<UnitTypeDto> Handle(CreateUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var codeExists = await _context.UnitTypes
            .AnyAsync(ut => ut.Code.ToLower() == request.Code.ToLower(), cancellationToken);

        if (codeExists)
        {
            throw new InvalidOperationException($"A Unit Type with code '{request.Code}' already exists.");
        }

        string? savedImagePath = null;
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

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.FloorPlanImageFile.CopyToAsync(stream, cancellationToken);
            }

            savedImagePath = $"/uploads/unit-types/{uniqueFileName}";
        }

        var entity = new UnitType
        {
            TypeName = request.TypeName,
            Code = request.Code,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            ParkingSlots = request.ParkingSlots,
            Floors = request.Floors,
            BuildingSize = request.BuildingSize,
            LandSizeRecommended = request.LandSizeRecommended,
            FloorPlanImageUrl = savedImagePath,
            SpecificationJson = request.SpecificationJson
        };

        _context.UnitTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-types");

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
            TotalUnitsCount = 0
        };
    }
}
