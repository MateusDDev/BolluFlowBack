using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.ExcludeFinancialEntry;

public class ExcludeFinancialEntryHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ExcludeFinancialEntryCommand, ResultOf<FinancialEntryModel>>
{
    public async Task<ResultOf<FinancialEntryModel>> Handle(ExcludeFinancialEntryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return ResultOf<FinancialEntryModel>.BadRequest(FinancialErrors.ExclusionReasonRequired);

            var entry = await db.FinancialEntries.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == request.EntryId, cancellationToken);
            if (entry is null)
                return ResultOf<FinancialEntryModel>.NotFound(FinancialErrors.EntryNotFound);

            if (entry.Status == Domain.Enums.FinancialEntryStatus.Excluded)
                return ResultOf<FinancialEntryModel>.BadRequest(FinancialErrors.EntryAlreadyExcluded);

            entry.Exclude();
            db.Set<FinancialEntryExclusion>().Add(FinancialEntryExclusion.Create(entry.Id, user.Id, request.Reason, request.PreserveSource));
            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<FinancialEntryModel>.Success(entry.ToModel());
        }
        catch (DomainException ex)
        {
            return ResultOf<FinancialEntryModel>.BadRequest(ex.Error);
        }
    }
}
