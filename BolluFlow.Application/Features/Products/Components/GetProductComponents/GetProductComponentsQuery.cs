using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Products.Components.GetProductComponents;

public record GetProductComponentsQuery(long ProductId) : IQuery<ResultOf<List<ProductComponentModel>>>;
