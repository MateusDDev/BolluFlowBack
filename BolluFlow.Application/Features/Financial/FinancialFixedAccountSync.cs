using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial;

public static class FinancialFixedAccountSync
{
    public static async Task EnsureEntriesForMonth(ApplicationDbContext db, long userId, FinancialMonth month, CancellationToken cancellationToken)
    {
        var accounts = await db.FixedAccounts
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        var applicableAccounts = accounts.Where(account => account.AppliesTo(month.Start)).ToList();
        if (applicableAccounts.Count == 0)
            return;

        var existingKeys = await db.FinancialEntries
            .Where(x => x.UserId == userId && x.Source == FinancialEntrySource.AutoFixed && x.ReferenceId != null && x.Date >= month.Start && x.Date <= month.End)
            .Select(x => x.ReferenceId!.Value)
            .ToListAsync(cancellationToken);

        var newEntries = applicableAccounts
            .Where(account => !existingKeys.Contains(account.Id))
            .Select(account => FinancialEntry.CreateAutomatic(
                userId,
                account.Description,
                FinancialEntryType.Expense,
                account.Category,
                account.Amount,
                month.Start,
                account.DefaultStatus,
                true,
                "Lançamento recorrente de conta fixa",
                FinancialEntrySource.AutoFixed,
                FinancialReferenceType.FixedAccount,
                account.Id))
            .ToList();

        if (newEntries.Count == 0)
            return;

        db.FinancialEntries.AddRange(newEntries);
        await db.SaveChangesAsync(cancellationToken);
    }
}
