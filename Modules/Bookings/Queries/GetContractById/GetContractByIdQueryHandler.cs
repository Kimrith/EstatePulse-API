using System.Data;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Contract;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, ContractDto?>
{
    private readonly AppDbContext _context;

    public GetContractByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ContractDto?> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT TOP(1)
                c.[id],
                c.[booking_id],
                c.[contract_number],
                c.[document_url],
                c.[status],
                c.[signed_at],
                c.[created_at],
                b.[booking_reference],
                u.[unit_code],
                usr.[full_name] AS [customer_name]
            FROM [Contracts] c
            INNER JOIN [Bookings] b ON c.[booking_id] = b.[id]
            INNER JOIN [Units] u ON b.[unit_id] = u.[id]
            INNER JOIN [Users] usr ON b.[customer_id] = usr.[id]
            WHERE c.[id] = @id;";

        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return ContractQueryMapping.ReadContractDto(reader);
        }

        return null;
    }
}
