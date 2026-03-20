using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Features.Financial;

public static class FinancialMapping
{
    public static FinancialEntryModel ToModel(this FinancialEntry entry) => new()
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
        Source = entry.Source,
        ReferenceType = entry.ReferenceType,
        ReferenceId = entry.ReferenceId
    };

    public static FixedAccountModel ToModel(this FixedAccount account) => new()
    {
        Id = account.Id,
        Description = account.Description,
        Category = account.Category,
        Amount = account.Amount,
        StartMonth = account.StartMonth.ToString("yyyy-MM"),
        EndMonth = account.EndMonth?.ToString("yyyy-MM"),
        DefaultStatus = account.DefaultStatus,
        IsDeleted = account.IsDeleted
    };
}
