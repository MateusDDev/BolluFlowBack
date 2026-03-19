using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.DeleteFinancialEntry;

public class DeleteFinancialEntryHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<DeleteFinancialEntryCommand, Result>
{
    public async Task<Result> Handle(DeleteFinancialEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await db.FinancialEntries
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == request.EntryId, cancellationToken);

        if (entry is null)
            return Result.NotFound(FinancialErrors.EntryNotFound);

        if (entry.Source != FinancialEntrySource.Manual)
            return Result.BadRequest(FinancialErrors.CannotDeleteAutomaticEntry);

        db.FinancialEntries.Remove(entry);
        await db.SaveChangesAsync(cancellationToken);

        return Result.NoContent();
    }
}
