using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Finance.ListFinancialEntries;

public record ListFinancialEntriesQuery(string? Type) : IQuery<ResultOf<List<FinancialEntryModel>>>;
