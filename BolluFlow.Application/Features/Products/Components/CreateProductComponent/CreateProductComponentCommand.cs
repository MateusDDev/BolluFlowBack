using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Products.Components.CreateProductComponent;

public record CreateProductComponentCommand(long ProductId, CreateProductComponentCommandBody Body) : ICommand<Result>;

public record CreateProductComponentCommandBody(ProductComponentType? ComponentType, long? RecipeId, long? IngredientId, long? SupplyId, decimal QuantityUsed);
