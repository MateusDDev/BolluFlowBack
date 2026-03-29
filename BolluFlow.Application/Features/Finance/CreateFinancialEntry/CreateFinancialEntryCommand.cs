using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Finance.CreateFinancialEntry;

public class CreateFinancialEntryCommand : ICommand<ResultOf<FinancialEntryModel>>
{
    public string Type { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = null!;
}
