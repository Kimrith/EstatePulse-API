using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed class ReleaseUnitHoldCommandHandler : IRequestHandler<ReleaseUnitHoldCommand, UnitDto>
{
    private readonly AppDbContext _context;

    public ReleaseUnitHoldCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<UnitDto> Handle(ReleaseUnitHoldCommand request, CancellationToken cancellationToken)
    {
        var unit = await _context.Units
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (unit == null)
        {
            throw new KeyNotFoundException($"Unit with ID {request.Id} was not found.");
        }

        if (request.UserId.HasValue && unit.CurrentHoldUserId.HasValue &&
            unit.CurrentHoldUserId.Value != request.UserId.Value)
        {
            throw new InvalidOperationException("You cannot release a hold placed by another user.");
        }

        unit.Status = "Available";
        unit.HoldExpiresAt = null;
        unit.CurrentHoldUserId = null;
        unit.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

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
            HoldExpiresAt = null,
            CurrentHoldUserId = null,
            CurrentHoldUserName = null,
            HoldRemainingSeconds = null,
            SvgElementId = unit.SvgElementId,
            RowVersion = unit.RowVersion != null ? Convert.ToBase64String(unit.RowVersion) : null,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }
}
