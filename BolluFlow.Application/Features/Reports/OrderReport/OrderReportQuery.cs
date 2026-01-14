using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Reports.OrderReport;

public record OrderReportQuery(long OrderId) : IQuery<ResultOf<byte[]>>;