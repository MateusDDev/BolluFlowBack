using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Financial.GenerateFixedAccountEntries;

public record GenerateFixedAccountEntriesCommand(string Month) : ICommand<ResultOf<List<FinancialEntryModel>>>;
