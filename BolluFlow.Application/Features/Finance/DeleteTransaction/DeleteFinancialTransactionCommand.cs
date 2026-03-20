using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Finance.DeleteTransaction;

public record DeleteFinancialTransactionCommand(Guid TransactionId) : ICommand<ResultOf<object>>;
