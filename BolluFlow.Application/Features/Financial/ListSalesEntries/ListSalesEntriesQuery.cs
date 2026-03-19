using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Financial.ListSalesEntries;

public record ListSalesEntriesQuery(string Month, int Page = 1, int PageSize = 50)
    : IQuery<ResultOf<PagedResult<AutoFinancialEntryModel>>>;
