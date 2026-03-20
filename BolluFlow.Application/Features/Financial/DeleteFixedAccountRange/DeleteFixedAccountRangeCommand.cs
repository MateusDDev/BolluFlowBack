using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Financial.DeleteFixedAccountRange;

public record DeleteFixedAccountRangeCommand(long FixedAccountId, DeletionMode Mode, string? Month)
    : ICommand<ResultOf<FixedAccountModel>>;
