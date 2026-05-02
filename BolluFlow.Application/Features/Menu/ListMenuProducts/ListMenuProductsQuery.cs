using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Menu.ListMenuProducts;

public record ListMenuProductsQuery(string Slug) : IQuery<ResultOf<List<MenuProductModel>>>;
