using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Financial.CreateFixedAccount;

public record CreateFixedAccountCommand(
    string Description,
    string Category,
    decimal Amount,
    string StartMonth,
    FinancialEntryStatus DefaultStatus
) : ICommand<ResultOf<FixedAccountModel>>;
