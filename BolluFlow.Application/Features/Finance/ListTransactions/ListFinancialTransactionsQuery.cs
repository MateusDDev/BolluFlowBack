using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;

namespace BolluFlow.Application.Features.Finance.ListTransactions;

public record ListFinancialTransactionsQuery(string Month) : IQuery<ResultOf<List<FinancialTransactionDto>>>;
