using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed record UpdateUnitMediaCommand(
    int Id,
    int? UnitId = null,
    int? UnitTypeId = null,
    string? MediaType = null,
    IFormFile? MediaFile = null,
    string? MediaUrl = null,
    int? SortOrder = null,
    bool? IsFeatured = null
) : IRequest<UnitMediaDto?>;
