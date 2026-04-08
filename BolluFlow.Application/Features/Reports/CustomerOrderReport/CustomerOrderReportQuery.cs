using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Reports.CustomerOrderReport;

public record CustomerOrderReportQuery(long OrderId) : IQuery<ResultOf<byte[]>>;
