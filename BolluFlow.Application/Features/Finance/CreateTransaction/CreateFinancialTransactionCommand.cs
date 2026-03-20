using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;

namespace BolluFlow.Application.Features.Finance.CreateTransaction;

public record CreateFinancialTransactionCommand(
    string Type,
    string Description,
    decimal Amount,
    string EventDate,
    string? Notes) : ICommand<ResultOf<FinancialTransactionDto>>;
