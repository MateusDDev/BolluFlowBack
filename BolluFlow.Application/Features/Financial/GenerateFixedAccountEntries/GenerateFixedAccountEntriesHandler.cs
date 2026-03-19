using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.GenerateFixedAccountEntries;

public class GenerateFixedAccountEntriesHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<GenerateFixedAccountEntriesCommand, ResultOf<List<FinancialEntryModel>>>
{
    public async Task<ResultOf<List<FinancialEntryModel>>> Handle(GenerateFixedAccountEntriesCommand request, CancellationToken cancellationToken)
    {
        if (!FinancialMonth.TryParse(request.Month, out var month))
            return ResultOf<List<FinancialEntryModel>>.BadRequest(FinancialErrors.InvalidMonthFormat);

        var accounts = await db.FixedAccounts
            .Where(x => x.UserId == user.Id && x.IsActive)
            .ToListAsync(cancellationToken);

        var existingReferenceIds = await db.FinancialEntries
            .Where(x => x.UserId == user.Id
                && x.Source == FinancialEntrySource.AutoFixedAccount
                && x.Date >= month.Start
                && x.Date <= month.End
                && x.ReferenceId != null)
            .Select(x => x.ReferenceId!.Value)
            .ToListAsync(cancellationToken);

        var newEntries = accounts
            .Where(a => !existingReferenceIds.Contains(a.Id))
            .Select(account => FinancialEntry.CreateAutomatic(
                user.Id,
                account.Description,
                FinancialEntryType.Expense,
                account.Category,
                account.DefaultAmount,
                new DateOnly(month.Start.Year, month.Start.Month, account.DueDay),
                FinancialEntryStatus.Pending,
                true,
                "Lançamento automático de conta fixa",
                FinancialEntrySource.AutoFixedAccount,
                account.Id))
            .ToList();

        if (newEntries.Count > 0)
        {
            db.FinancialEntries.AddRange(newEntries);
            await db.SaveChangesAsync(cancellationToken);
        }

        return ResultOf<List<FinancialEntryModel>>.Success(newEntries.Select(x => x.ToModel()).ToList());
    }
}
