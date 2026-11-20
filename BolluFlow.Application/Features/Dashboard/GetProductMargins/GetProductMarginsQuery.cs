using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetProductMargins;

public sealed record GetProductMarginsQuery : IQuery<ResultOf<List<GetProductMarginsResult>>>;

public sealed record GetProductMarginsResult(
    long Id,
    string Name,
    decimal Price,
    decimal Cost,
    decimal? Margin,
    bool HasRecipe);
