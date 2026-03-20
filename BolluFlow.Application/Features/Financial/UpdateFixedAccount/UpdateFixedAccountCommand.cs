using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Financial.UpdateFixedAccount;

public record UpdateFixedAccountCommand(
    long FixedAccountId,
    string Description,
    string Category,
    decimal Amount,
    string StartMonth,
    FinancialEntryStatus DefaultStatus
) : ICommand<ResultOf<FixedAccountModel>>;
