using BolluFlow.Application.Abstractions;
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
        if (!DateOnly.TryParse($"{request.Month}-01", out var monthDate))
            return ResultOf<List<FinancialEntryModel>>.BadRequest(FinancialErrors.InvalidMonthFormat);

        var accounts = await db.FixedAccounts
            .Where(x => x.UserId == user.Id && x.IsActive)
            .ToListAsync(cancellationToken);

        var startMonth = new DateOnly(monthDate.Year, monthDate.Month, 1);
        var endMonth = startMonth.AddMonths(1).AddDays(-1);

        var existingDescriptionsInMonth = await db.FinancialEntries
            .Where(x => x.UserId == user.Id && x.IsFixed && x.Date >= startMonth && x.Date <= endMonth)
            .Select(x => x.Description)
            .ToListAsync(cancellationToken);

        var newEntries = accounts
            .Where(a => !existingDescriptionsInMonth.Contains(a.Description))
            .Select(account => FinancialEntry.Create(
                user.Id,
                account.Description,
                FinancialEntryType.Expense,
                account.Category,
                account.DefaultAmount,
                new DateOnly(monthDate.Year, monthDate.Month, account.DueDay),
                FinancialEntryStatus.Pending,
                true,
                "Lançamento automático de conta fixa"))
            .ToList();

        if (newEntries.Count > 0)
        {
            db.FinancialEntries.AddRange(newEntries);
            await db.SaveChangesAsync(cancellationToken);
        }

        return ResultOf<List<FinancialEntryModel>>.Success(newEntries.Select(x => new FinancialEntryModel
        {
            Id = x.Id,
            Description = x.Description,
            Type = x.Type,
            Category = x.Category,
            Amount = x.Amount,
            Date = x.Date,
            Status = x.Status,
            IsFixed = x.IsFixed,
            Notes = x.Notes,
            IsAutomaticFromSale = x.IsAutomaticFromSale,
            SourceOrderId = x.SourceOrderId
        }).ToList());
    }
}
