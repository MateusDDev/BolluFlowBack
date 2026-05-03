using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class Product
{
    private Product() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Category { get; private set; }
    public decimal? SalePrice { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal? MinimumStockQuantity { get; private set; }
    public decimal? IndirectCostPercentage { get; private set; }
    public decimal MarkupPercentage { get; private set; }
    public decimal FinalPrice { get; private set; }
    public decimal? DiscountPercentage { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    public List<ProductComponent> Components { get; private set; } = [];

    public static Product Create(long userId, string name, string? category, decimal? salePrice, decimal stockQuantity, decimal? minimumStockQuantity)
    {
        Validate(name, stockQuantity, minimumStockQuantity);

        return new Product
        {
            UserId = userId,
            Name = name,
            Category = NormalizeCategory(category),
            SalePrice = salePrice,
            StockQuantity = stockQuantity,
            MinimumStockQuantity = minimumStockQuantity,
            IndirectCostPercentage = null,
            MarkupPercentage = 0,
            FinalPrice = salePrice ?? 0,
            DiscountPercentage = null,
            ImageUrl = null,
            IsActive = true
        };
    }

    public void Update(string name, string? category, decimal? salePrice, decimal stockQuantity, decimal? minimumStockQuantity, bool? isActive)
    {
        Validate(name, stockQuantity, minimumStockQuantity);

        Name = name;
        Category = NormalizeCategory(category);
        SalePrice = salePrice;
        StockQuantity = stockQuantity;
        MinimumStockQuantity = minimumStockQuantity;
        IsActive = isActive ?? IsActive;
    }

    public void UpdatePricing(decimal? indirectCostPercentage, decimal markupPercentage, decimal finalPrice, decimal? discountPercentage)
    {
        if (finalPrice < 0)
            throw new DomainException(ProductErrors.FinalPriceInvalid);
        if (discountPercentage is < 0 or > 100)
            throw new DomainException(ProductErrors.InvalidDiscount);

        if (discountPercentage.HasValue)
            finalPrice -= finalPrice * (discountPercentage.Value / 100);

        IndirectCostPercentage = indirectCostPercentage;
        MarkupPercentage = markupPercentage;
        FinalPrice = finalPrice;
        DiscountPercentage = discountPercentage;
    }

    public void UpdateImage(string? imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void ConsumeStock(decimal quantity)
    {
        if (quantity <= 0 || StockQuantity < quantity)
            throw new DomainException(ProductErrors.StockInsufficient(Name));

        StockQuantity -= quantity;
    }

    public void AddStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(ProductErrors.StockQuantityInvalid);

        StockQuantity += quantity;
    }

    private static void Validate(string name, decimal stockQuantity, decimal? minimumStockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(ProductErrors.NameRequired);
        if (stockQuantity < 0)
            throw new DomainException(ProductErrors.StockQuantityInvalid);
        if (minimumStockQuantity < 0)
            throw new DomainException(ProductErrors.MinimumStockQuantityInvalid);
    }

    private static string? NormalizeCategory(string? category)
    {
        return string.IsNullOrWhiteSpace(category) ? null : category.Trim();
    }
}
