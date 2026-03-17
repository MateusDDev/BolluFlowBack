using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Financial.UpdateFinancialEntryStatus;

public record UpdateFinancialEntryStatusCommand(long EntryId, FinancialEntryStatus Status) : ICommand<ResultOf<FinancialEntryModel>>;
