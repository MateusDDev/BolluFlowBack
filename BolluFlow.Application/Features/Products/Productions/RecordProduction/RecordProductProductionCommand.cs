using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Products.Productions.RecordProduction;

public record RecordProductProductionCommand(long ProductId, RecordProductProductionCommandBody Body) : ICommand<ResultOf<ProductProductionModel>>;

public record RecordProductProductionCommandBody(decimal Quantity, string? Notes);
