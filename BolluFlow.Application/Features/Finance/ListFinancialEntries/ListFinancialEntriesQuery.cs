using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Finance.ListFinancialEntries;

public record ListFinancialEntriesQuery(string? Type, int? Month, int? Year) : IQuery<ResultOf<List<FinancialEntryModel>>>;
