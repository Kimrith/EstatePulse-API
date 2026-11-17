using EstatePulse.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Delete
{
    public record DeleteUserFollowsCommand(int Id) : IRequest<bool>;

    public class DeleteUserFollowsCommandHandler : IRequestHandler<DeleteUserFollowsCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteUserFollowsCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<bool> Handle(DeleteUserFollowsCommand request, CancellationToken cancellationToken)
        {
            // In EF Core 7+, ExecuteDeleteAsync executes directly in the database without loading the entity into memory
            int rowsAffected = await _context.UserFollows
                .Where(uf => uf.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            return rowsAffected > 0;
        }
    }
}