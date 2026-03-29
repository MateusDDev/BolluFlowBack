using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using MediatR;

namespace BolluFlow.Application.Features.Finance.GetFinancialBalance;

public record GetFinancialBalanceQuery() : IQuery<ResultOf<FinancialBalanceModel>>;
