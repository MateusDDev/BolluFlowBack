using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Inventory.CreatePurchase;

public record CreatePurchaseCommand(
    string ItemType,
    long ItemId,
    string? ItemName,
    string ConversionName,
    decimal Quantity,
    decimal QuantityInBaseUnit,
    decimal TotalCost,
    string PurchasedAt,
    string? Notes) : ICommand<ResultOf<object>>;
