using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Financial.CreateFinancialEntry;

public record CreateFinancialEntryCommand(
    string Description,
    FinancialEntryType Type,
    string Category,
    decimal Amount,
    DateOnly Date,
    FinancialEntryStatus Status,
    bool IsFixed,
    string? Notes
) : ICommand<ResultOf<FinancialEntryModel>>;
