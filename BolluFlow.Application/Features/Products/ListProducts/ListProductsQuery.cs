using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Products.ListProducts;

public record ListProductsQuery : IQuery<ResultOf<List<ProductModel>>>;
