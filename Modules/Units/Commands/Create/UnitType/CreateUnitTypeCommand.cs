using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Units.Commands.Create;

public sealed record CreateUnitTypeCommand(
    string TypeName,
    string Code,
    int Bedrooms,
    int Bathrooms,
    int ParkingSlots,
    int Floors,
    decimal BuildingSize,
    decimal? LandSizeRecommended,
    IFormFile? FloorPlanImageFile,
    string? SpecificationJson
) : IRequest<UnitTypeDto>;
