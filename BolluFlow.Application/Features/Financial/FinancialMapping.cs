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
        ReferenceId = entry.ReferenceId
    };
}
