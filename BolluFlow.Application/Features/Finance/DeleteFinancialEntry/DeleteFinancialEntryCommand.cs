using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Finance.DeleteFinancialEntry;

public record DeleteFinancialEntryCommand(long Id, bool RevertStock = false) : ICommand<Result>;
