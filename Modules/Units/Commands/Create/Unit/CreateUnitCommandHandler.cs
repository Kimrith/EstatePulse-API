using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using EstatePulse.Modules.Units.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Unit = EstatePulse.Modules.Units.Models.Unit;

namespace EstatePulse.Modules.Units.Commands.Create;

public sealed class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, UnitDto>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public CreateUnitCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<UnitDto> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        var phase = await _context.ProjectPhases
            .FirstOrDefaultAsync(p => p.Id == request.PhaseId, cancellationToken);

        if (phase == null)
        {
            throw new KeyNotFoundException($"Project phase with ID {request.PhaseId} was not found.");
        }

        var unitType = await _context.UnitTypes
            .FirstOrDefaultAsync(ut => ut.Id == request.UnitTypeId, cancellationToken);

        if (unitType == null)
        {
            throw new KeyNotFoundException($"Unit type with ID {request.UnitTypeId} was not found.");
        }

        var codeExists = await _context.Units
            .AnyAsync(u => u.PhaseId == request.PhaseId && u.UnitCode.ToLower() == request.UnitCode.ToLower(), cancellationToken);

        if (codeExists)
        {
            throw new InvalidOperationException($"A unit with code '{request.UnitCode}' already exists in phase '{phase.PhaseName}'.");
        }

        var unit = new Unit
        {
            PhaseId = request.PhaseId,
            UnitTypeId = request.UnitTypeId,
            UnitCode = request.UnitCode,
            StreetNumber = request.StreetNumber,
            LandAreaSqm = request.LandAreaSqm,
            BuiltAreaSqm = request.BuiltAreaSqm,
            BasePrice = request.BasePrice,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Available" : request.Status,
            SvgElementId = request.SvgElementId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Units.Add(unit);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-types");

        var projectName = await _context.Projects
            .Where(p => p.Id == phase.ProjectId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return new UnitDto
        {
            Id = unit.Id,
            PhaseId = unit.PhaseId,
            PhaseName = phase.PhaseName,
            ProjectId = phase.ProjectId,
            ProjectName = projectName,
            UnitTypeId = unit.UnitTypeId,
            UnitTypeCode = unitType.Code,
            UnitTypeName = unitType.TypeName,
            UnitCode = unit.UnitCode,
            StreetNumber = unit.StreetNumber,
            LandAreaSqm = unit.LandAreaSqm,
            BuiltAreaSqm = unit.BuiltAreaSqm,
            BasePrice = unit.BasePrice,
            Status = unit.Status,
            SvgElementId = unit.SvgElementId,
            RowVersion = unit.RowVersion != null ? Convert.ToBase64String(unit.RowVersion) : null,
            CreatedAt = unit.CreatedAt
        };
    }
}
