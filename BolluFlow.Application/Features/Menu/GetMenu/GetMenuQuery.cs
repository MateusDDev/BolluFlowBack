using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Menu.GetMenu;

public record GetMenuQuery(string Slug) : IQuery<ResultOf<MenuStoreModel>>;
