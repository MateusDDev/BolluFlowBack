using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Financial.ListSalesEntries;

public record ListSalesEntriesQuery(
    DateOnly? StartDate,
    DateOnly? EndDate,
    int Page = 1,
    int PageSize = 20
) : IQuery<ResultOf<PagedResult<SaleFinancialEntryModel>>>;
