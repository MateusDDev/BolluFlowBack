using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Finance.GetFinancialBalance;

public record GetFinancialBalanceQuery(int? Month, int? Year) : IQuery<ResultOf<FinancialBalanceModel>>;
