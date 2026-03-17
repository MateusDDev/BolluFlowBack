using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Financial.GetFinancialOverview;

public record GetFinancialOverviewQuery(string Month) : IQuery<ResultOf<FinancialOverviewModel>>;
