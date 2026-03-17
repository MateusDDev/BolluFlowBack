using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Financial.ListFinancialEntries;

public record ListFinancialEntriesQuery(
    DateOnly? StartDate,
    DateOnly? EndDate,
    FinancialEntryType? Type,
    FinancialEntryStatus? Status,
    bool? IsFixed,
    int Page = 1,
    int PageSize = 20
) : IQuery<ResultOf<PagedResult<FinancialEntryModel>>>;
