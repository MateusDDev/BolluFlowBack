using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Products.AdjustStock;

public record AdjustProductStockCommand(long ProductId, AdjustProductStockCommandBody Body) : ICommand<ResultOf<ProductStockAdjustmentModel>>;

public record AdjustProductStockCommandBody(decimal Quantity, string Direction, string Reason);
