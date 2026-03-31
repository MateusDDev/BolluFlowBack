namespace BolluFlow.Domain.Entities;

public class ProductProduction
{
    private ProductProduction() { }

    public long Id { get; private set; }
    public long ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static ProductProduction Create(long productId, decimal quantity, string? notes)
    {
        return new ProductProduction
        {
            ProductId = productId,
            Quantity = quantity,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }
}
