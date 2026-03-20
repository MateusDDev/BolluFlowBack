using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.UpdateFinancialEntryStatus;

public class UpdateFinancialEntryStatusHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<UpdateFinancialEntryStatusCommand, ResultOf<FinancialEntryModel>>
{
    public async Task<ResultOf<FinancialEntryModel>> Handle(UpdateFinancialEntryStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entry = await db.FinancialEntries.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == request.EntryId, cancellationToken);
            if (entry is null)
                return ResultOf<FinancialEntryModel>.NotFound(FinancialErrors.EntryNotFound);

            entry.UpdateStatus(request.Status);
            await db.SaveChangesAsync(cancellationToken);
            return ResultOf<FinancialEntryModel>.Success(entry.ToModel());
        }
        catch (DomainException ex)
        {
            return ResultOf<FinancialEntryModel>.BadRequest(ex.Error);
        }
    }
}
