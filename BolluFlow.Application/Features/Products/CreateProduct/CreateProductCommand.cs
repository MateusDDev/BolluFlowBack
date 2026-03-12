using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Products.CreateProduct;

public record CreateProductCommand(string Name, decimal? SalePrice, decimal StockQuantity, decimal? MinimumStockQuantity)
    : ICommand<ResultOf<CreateProductResult>>;

public record CreateProductResult(long Id);
