using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Financial.CreateFinancialEntry;

public class CreateFinancialEntryHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateFinancialEntryCommand, ResultOf<FinancialEntryModel>>
{
    public async Task<ResultOf<FinancialEntryModel>> Handle(CreateFinancialEntryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entry = FinancialEntry.CreateManual(
                user.Id,
                request.Description,
                request.Type,
                request.Category,
                request.Amount,
                request.Date,
                request.Status,
                request.IsFixed,
                request.Notes);

            db.FinancialEntries.Add(entry);
            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<FinancialEntryModel>.Success(entry.ToModel());
        }
        catch (DomainException ex)
        {
            return ResultOf<FinancialEntryModel>.BadRequest(ex.Error);
        }
    }
}
