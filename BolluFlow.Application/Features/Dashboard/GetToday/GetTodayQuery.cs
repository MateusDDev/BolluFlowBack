using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetToday;

public sealed record GetTodayQuery(DateTime DayStart) : IQuery<ResultOf<GetTodayResult>>;
