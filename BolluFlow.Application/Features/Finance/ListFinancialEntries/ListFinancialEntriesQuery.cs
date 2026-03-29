using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using MediatR;

namespace BolluFlow.Application.Features.Finance.ListFinancialEntries;

public record ListFinancialEntriesQuery(string? Tipo) : IQuery<ResultOf<List<FinancialEntryModel>>>;
