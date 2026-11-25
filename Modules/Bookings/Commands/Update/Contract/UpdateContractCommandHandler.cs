using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Contract;
using EstatePulse.Modules.Bookings.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed class UpdateContractCommandHandler : IRequestHandler<UpdateContractCommand, ContractDto?>
{
    private readonly AppDbContext _context;

    public UpdateContractCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ContractDto?> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _context.Contracts
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (contract == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.ContractNumber))
        {
            var trimmedNumber = request.ContractNumber.Trim();
            if (!string.Equals(trimmedNumber, contract.ContractNumber, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _context.Contracts
                    .AnyAsync(c => c.ContractNumber == trimmedNumber && c.Id != request.Id, cancellationToken);

                if (exists)
                {
                    throw new InvalidOperationException($"A contract with number '{trimmedNumber}' already exists.");
                }

                contract.ContractNumber = trimmedNumber;
            }
        }

        if (request.DocumentUrl != null)
        {
            contract.DocumentUrl = string.IsNullOrWhiteSpace(request.DocumentUrl) ? null : request.DocumentUrl.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            contract.Status = request.Status.Trim();
        }

        if (request.SignedAt.HasValue)
        {
            contract.SignedAt = request.SignedAt.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await BuildContractDtoAsync(_context, contract, cancellationToken);
    }

    internal static async Task<ContractDto> BuildContractDtoAsync(
        AppDbContext context,
        Contract contract,
        CancellationToken cancellationToken)
    {
        var bookingInfo = await context.Bookings
            .Where(b => b.Id == contract.BookingId)
            .Select(b => new { b.BookingReference, b.UnitId, b.CustomerId })
            .FirstOrDefaultAsync(cancellationToken);

        string? unitCode = null;
        string? customerName = null;
        if (bookingInfo != null)
        {
            unitCode = await context.Units
                .Where(u => u.Id == bookingInfo.UnitId)
                .Select(u => u.UnitCode)
                .FirstOrDefaultAsync(cancellationToken);

            customerName = await context.Users
                .Where(u => u.Id == bookingInfo.CustomerId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new ContractDto
        {
            Id = contract.Id,
            BookingId = contract.BookingId,
            ContractNumber = contract.ContractNumber,
            DocumentUrl = contract.DocumentUrl,
            Status = contract.Status,
            SignedAt = contract.SignedAt,
            CreatedAt = contract.CreatedAt,
            BookingReference = bookingInfo?.BookingReference,
            UnitCode = unitCode,
            CustomerName = customerName
        };
    }
}
