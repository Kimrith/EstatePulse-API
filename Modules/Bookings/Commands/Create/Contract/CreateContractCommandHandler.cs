using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Contract;
using EstatePulse.Modules.Bookings.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Create;

public sealed class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, ContractDto>
{
    private readonly AppDbContext _context;

    public CreateContractCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ContractDto> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking with ID {request.BookingId} does not exist.");
        }

        string contractNumber = string.IsNullOrWhiteSpace(request.ContractNumber)
            ? $"CT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}"
            : request.ContractNumber.Trim();

        var numberExists = await _context.Contracts
            .AnyAsync(c => c.ContractNumber == contractNumber, cancellationToken);

        if (numberExists)
        {
            throw new InvalidOperationException($"A contract with number '{contractNumber}' already exists.");
        }

        var status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim();

        var contract = new Contract
        {
            BookingId = request.BookingId,
            ContractNumber = contractNumber,
            DocumentUrl = string.IsNullOrWhiteSpace(request.DocumentUrl) ? null : request.DocumentUrl.Trim(),
            Status = status,
            SignedAt = string.Equals(status, "Signed", StringComparison.OrdinalIgnoreCase) ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow
        };

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync(cancellationToken);

        var unitCode = await _context.Units
            .Where(u => u.Id == booking.UnitId)
            .Select(u => u.UnitCode)
            .FirstOrDefaultAsync(cancellationToken);

        var customerName = await _context.Users
            .Where(u => u.Id == booking.CustomerId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync(cancellationToken);

        return new ContractDto
        {
            Id = contract.Id,
            BookingId = contract.BookingId,
            ContractNumber = contract.ContractNumber,
            DocumentUrl = contract.DocumentUrl,
            Status = contract.Status,
            SignedAt = contract.SignedAt,
            CreatedAt = contract.CreatedAt,
            BookingReference = booking.BookingReference,
            UnitCode = unitCode,
            CustomerName = customerName
        };
    }
}
