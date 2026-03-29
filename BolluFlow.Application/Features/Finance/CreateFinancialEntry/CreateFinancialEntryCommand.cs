using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Finance.CreateFinancialEntry;

public record CreateFinancialEntryCommand(
    FinancialEntryType Type,
    decimal Amount,
    string Description
) : ICommand<ResultOf<FinancialEntryModel>>;
