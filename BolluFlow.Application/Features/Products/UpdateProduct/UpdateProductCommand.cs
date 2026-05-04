using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Products.UpdateProduct;

public record UpdateProductCommand(long Id, UpdateProductCommandBody Body) : ICommand<ResultOf<ProductModel>>;

public class UpdateProductCommandBody
{
    public string? Name { get; init; }
    public string? Category { get; init; }
    public OptionalField<string?> Description { get; init; }
    public decimal? SalePrice { get; init; }
    public decimal? MinimumStockQuantity { get; init; }
    public bool? IsActive { get; init; }

    public OptionalField<string?> ImageUrl { get; init; }
}
