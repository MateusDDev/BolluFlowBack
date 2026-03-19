using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Financial.ListFinancialEntries;

public record ListFinancialEntriesQuery(
    string Month,
    FinancialEntryType? Type,
    FinancialEntryStatus? Status,
    int Page = 1,
    int PageSize = 50
) : IQuery<ResultOf<PagedResult<FinancialEntryModel>>>;
