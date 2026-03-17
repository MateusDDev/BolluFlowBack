using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
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
        var entry = await db.FinancialEntries
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == request.EntryId, cancellationToken);

        if (entry is null)
            return ResultOf<FinancialEntryModel>.NotFound(FinancialErrors.EntryNotFound);

        entry.UpdateStatus(request.Status);
        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<FinancialEntryModel>.Success(ToModel(entry));
    }

    private static FinancialEntryModel ToModel(FinancialEntry entry) => new()
    {
        Id = entry.Id,
        Description = entry.Description,
        Type = entry.Type,
        Category = entry.Category,
        Amount = entry.Amount,
        Date = entry.Date,
        Status = entry.Status,
        IsFixed = entry.IsFixed,
        Notes = entry.Notes,
        IsAutomaticFromSale = entry.IsAutomaticFromSale,
        SourceOrderId = entry.SourceOrderId
    };
}
