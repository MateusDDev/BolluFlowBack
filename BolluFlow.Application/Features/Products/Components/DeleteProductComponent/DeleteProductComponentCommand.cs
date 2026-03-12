using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Products.Components.DeleteProductComponent;

public record DeleteProductComponentCommand(long ProductId, long ComponentId) : ICommand<Result>;
