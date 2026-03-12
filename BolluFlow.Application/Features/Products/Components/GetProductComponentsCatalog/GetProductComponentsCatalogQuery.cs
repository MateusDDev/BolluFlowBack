using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Products.Components.GetProductComponentsCatalog;

public record GetProductComponentsCatalogQuery(long ProductId) : IQuery<ResultOf<List<ProductComponentCatalogItemModel>>>;
