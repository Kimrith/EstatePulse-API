using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed record UpdateUnitTypeCommand(
    int Id,
    string? TypeName = null,
    string? Code = null,
    int? Bedrooms = null,
    int? Bathrooms = null,
    int? ParkingSlots = null,
    int? Floors = null,
    decimal? BuildingSize = null,
    decimal? LandSizeRecommended = null,
    IFormFile? FloorPlanImageFile = null,
    string? SpecificationJson = null
) : IRequest<UnitTypeDto?>;
