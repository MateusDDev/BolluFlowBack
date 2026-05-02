using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Menu.CreateMenuOrder;

public record CreateMenuOrderCommand(
    string Slug,
    string? CustomerName,
    string? CustomerWhatsapp,
    string? CustomerStreet,
    int? CustomerNumber,
    string? CustomerNeighborhood,
    string? Note,
    List<CreateMenuOrderItemBody>? Items) : ICommand<ResultOf<MenuOrderCreatedModel>>;

public record CreateMenuOrderRequestBody(
    string? CustomerName,
    string? CustomerWhatsapp,
    string? CustomerStreet,
    int? CustomerNumber,
    string? CustomerNeighborhood,
    string? Note,
    List<CreateMenuOrderItemBody>? Items);

public record CreateMenuOrderItemBody(long ProductId, int Quantity);


