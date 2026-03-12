using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Products.GetProductDetails;

public record GetProductDetailsQuery(long Id) : IQuery<ResultOf<ProductModel>>;
