using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Units.Commands.Create;

public sealed record CreateUnitMediaCommand(
    int? UnitId,
    int? UnitTypeId,
    string MediaType,
    IFormFile? MediaFile,
    string? MediaUrl,
    int SortOrder,
    bool IsFeatured
) : IRequest<UnitMediaDto>;
