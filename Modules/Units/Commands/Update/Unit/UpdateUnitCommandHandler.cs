using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, UnitDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public UpdateUnitCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<UnitDto?> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _context.Units
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (unit == null)
        {
            return null;
        }

        int targetPhaseId = request.PhaseId ?? unit.PhaseId;

        if (request.PhaseId.HasValue && request.PhaseId.Value != unit.PhaseId)
        {
            var phaseExists = await _context.ProjectPhases.AnyAsync(p => p.Id == request.PhaseId.Value, cancellationToken);
            if (!phaseExists)
            {
                throw new KeyNotFoundException($"Project phase with ID {request.PhaseId.Value} was not found.");
            }
            unit.PhaseId = request.PhaseId.Value;
        }

        if (request.UnitTypeId.HasValue && request.UnitTypeId.Value != unit.UnitTypeId)
        {
            var unitTypeExists = await _context.UnitTypes.AnyAsync(ut => ut.Id == request.UnitTypeId.Value, cancellationToken);
            if (!unitTypeExists)
            {
                throw new KeyNotFoundException($"Unit type with ID {request.UnitTypeId.Value} was not found.");
            }
            unit.UnitTypeId = request.UnitTypeId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.UnitCode) && !string.Equals(request.UnitCode, unit.UnitCode, StringComparison.OrdinalIgnoreCase))
        {
            var codeTaken = await _context.Units
                .AnyAsync(u => u.PhaseId == targetPhaseId && u.UnitCode.ToLower() == request.UnitCode.ToLower() && u.Id != request.Id, cancellationToken);

            if (codeTaken)
            {
                throw new InvalidOperationException($"A unit with code '{request.UnitCode}' already exists in this phase.");
            }
            unit.UnitCode = request.UnitCode;
        }

        if (request.StreetNumber != null) unit.StreetNumber = request.StreetNumber;
        if (request.LandAreaSqm.HasValue) unit.LandAreaSqm = request.LandAreaSqm.Value;
        if (request.BuiltAreaSqm.HasValue) unit.BuiltAreaSqm = request.BuiltAreaSqm.Value;
        if (request.BasePrice.HasValue) unit.BasePrice = request.BasePrice.Value;
        if (!string.IsNullOrWhiteSpace(request.Status)) unit.Status = request.Status;
        if (request.SvgElementId != null) unit.SvgElementId = request.SvgElementId;
        unit.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-types");

        var phaseInfo = await _context.ProjectPhases
            .Where(p => p.Id == unit.PhaseId)
            .Select(p => new { p.PhaseName, p.ProjectId })
            .FirstOrDefaultAsync(cancellationToken);

        string? projectName = null;
        if (phaseInfo != null)
        {
            projectName = await _context.Projects
                .Where(p => p.Id == phaseInfo.ProjectId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var unitTypeInfo = await _context.UnitTypes
            .Where(ut => ut.Id == unit.UnitTypeId)
            .Select(ut => new { ut.Code, ut.TypeName })
            .FirstOrDefaultAsync(cancellationToken);

        string? currentHoldUserName = null;
        if (unit.CurrentHoldUserId.HasValue)
        {
            currentHoldUserName = await _context.Users
                .Where(u => u.Id == unit.CurrentHoldUserId.Value)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new UnitDto
        {
            Id = unit.Id,
            PhaseId = unit.PhaseId,
            PhaseName = phaseInfo?.PhaseName,
            ProjectId = phaseInfo?.ProjectId,
            ProjectName = projectName,
            UnitTypeId = unit.UnitTypeId,
            UnitTypeCode = unitTypeInfo?.Code,
            UnitTypeName = unitTypeInfo?.TypeName,
            UnitCode = unit.UnitCode,
            StreetNumber = unit.StreetNumber,
            LandAreaSqm = unit.LandAreaSqm,
            BuiltAreaSqm = unit.BuiltAreaSqm,
            BasePrice = unit.BasePrice,
            Status = unit.Status,
            HoldExpiresAt = unit.HoldExpiresAt,
            CurrentHoldUserId = unit.CurrentHoldUserId,
            CurrentHoldUserName = currentHoldUserName,
            HoldRemainingSeconds = unit.HoldExpiresAt.HasValue && unit.HoldExpiresAt.Value > DateTime.UtcNow
                ? (int)(unit.HoldExpiresAt.Value - DateTime.UtcNow).TotalSeconds
                : null,
            SvgElementId = unit.SvgElementId,
            RowVersion = unit.RowVersion != null ? Convert.ToBase64String(unit.RowVersion) : null,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }
}
