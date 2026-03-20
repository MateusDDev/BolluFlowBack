using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;

namespace BolluFlow.Application.Features.Finance.GetOverview;

public record GetFinanceOverviewQuery(string Month) : IQuery<ResultOf<FinanceOverviewDto>>;
