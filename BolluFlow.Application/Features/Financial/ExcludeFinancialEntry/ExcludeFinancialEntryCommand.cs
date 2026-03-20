using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Financial.ExcludeFinancialEntry;

public record ExcludeFinancialEntryCommand(long EntryId, string Reason, bool PreserveSource) : ICommand<ResultOf<FinancialEntryModel>>;
