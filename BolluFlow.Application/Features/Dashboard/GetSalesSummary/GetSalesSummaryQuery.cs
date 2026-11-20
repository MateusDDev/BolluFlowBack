using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetSalesSummary;

public sealed record GetSalesSummaryQuery(DateTime From, DateTime To, int UtcOffsetMinutes = 0, DateTime? PreviousFrom = null)
    : IQuery<ResultOf<GetSalesSummaryResult>>;
