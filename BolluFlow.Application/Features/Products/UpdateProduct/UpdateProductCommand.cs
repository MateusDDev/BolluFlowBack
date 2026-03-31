using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Products.UpdateProduct;

public record UpdateProductCommand(long Id, UpdateProductCommandBody Body) : ICommand<ResultOf<ProductModel>>;

public record UpdateProductCommandBody(string? Name, decimal? SalePrice, decimal? MinimumStockQuantity, bool? IsActive);
