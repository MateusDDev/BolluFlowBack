namespace BolluFlow.Domain.Abstractions;

public interface IStockItem
{
    long Id { get; }
    string Name { get; }
    decimal StockQuantity { get; }
    bool TrackStock { get; }

    bool HasEnoughStock(decimal quantity, bool allowNegativeStock);
    void ConsumeStock(decimal quantity, bool allowNegativeStock = false);
    void RevertStock(decimal quantity);
}
