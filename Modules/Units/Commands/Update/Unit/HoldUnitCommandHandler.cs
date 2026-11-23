using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed class HoldUnitCommandHandler : IRequestHandler<HoldUnitCommand, UnitDto>
{
    private readonly AppDbContext _context;

    public HoldUnitCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<UnitDto> Handle(HoldUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _context.Units
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (unit == null)
        {
            throw new KeyNotFoundException($"Unit with ID {request.Id} was not found.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} was not found.");
        }

        // 1. Optimistic Concurrency Check via RowVersion
        if (!string.IsNullOrEmpty(request.RowVersion) && unit.RowVersion != null)
        {
            byte[] clientBytes = Convert.FromBase64String(request.RowVersion);
            if (!unit.RowVersion.SequenceEqual(clientBytes))
            {
                throw new DbUpdateConcurrencyException("The unit was modified or claimed by another user. Please refresh and try again.");
            }
        }

        // 2. Status & Reservation Conflict Checks
        if (string.Equals(unit.Status, "Booked", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(unit.Status, "Sold", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unit {unit.UnitCode} is currently '{unit.Status}' and cannot be placed on hold.");
        }

        var now = DateTime.UtcNow;
        if (string.Equals(unit.Status, "OnHold", StringComparison.OrdinalIgnoreCase))
        {
            if (unit.HoldExpiresAt.HasValue && unit.HoldExpiresAt.Value > now && unit.CurrentHoldUserId != request.UserId)
            {
                throw new InvalidOperationException($"Unit {unit.UnitCode} is already on hold by another customer until {unit.HoldExpiresAt:u}.");
            }
        }

        // 3. Grant Hold
        int holdDurationMinutes = request.HoldMinutes > 0 ? request.HoldMinutes : 15;
        unit.Status = "OnHold";
        unit.CurrentHoldUserId = request.UserId;
        unit.HoldExpiresAt = now.AddMinutes(holdDurationMinutes);
        unit.UpdatedAt = now;

        await _context.SaveChangesAsync(cancellationToken);

        int remainingSeconds = (int)(unit.HoldExpiresAt.Value - DateTime.UtcNow).TotalSeconds;

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
            HoldExpiresAt = unit.HoldExpiresAt,
            CurrentHoldUserId = unit.CurrentHoldUserId,
            CurrentHoldUserName = user.FullName,
            HoldRemainingSeconds = remainingSeconds > 0 ? remainingSeconds : 0,
            SvgElementId = unit.SvgElementId,
            RowVersion = unit.RowVersion != null ? Convert.ToBase64String(unit.RowVersion) : null,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }
}
