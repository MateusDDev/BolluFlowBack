using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Products.Components.UpdateProductComponent;

public record UpdateProductComponentCommand(long ProductId, long ComponentId, UpdateProductComponentCommandBody Body) : ICommand<Result>;

public record UpdateProductComponentCommandBody(decimal QuantityUsed);
