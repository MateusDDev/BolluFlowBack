using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Financial.DeleteFinancialEntry;

public record DeleteFinancialEntryCommand(long EntryId) : ICommand<Result>;
