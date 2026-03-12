using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetLowStockSupplies;

public record GetLowStockSuppliesQuery : IQuery<ResultOf<List<GetLowStockSuppliesResult>>>;
