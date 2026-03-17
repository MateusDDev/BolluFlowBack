using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Financial.CreateFixedAccount;

public record CreateFixedAccountCommand(
    string Description,
    string Category,
    decimal DefaultAmount,
    int DueDay,
    bool IsActive
) : ICommand<ResultOf<FixedAccountModel>>;
