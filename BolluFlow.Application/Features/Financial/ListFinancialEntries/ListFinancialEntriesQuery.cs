using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Financial.ListFinancialEntries;

public record ListFinancialEntriesQuery(
    string Month,
    string? Type,
    string? Source,
    string? Status,
    int Page = 1,
    int PageSize = 50
) : IQuery<ResultOf<PagedResult<FinancialEntryModel>>>;
