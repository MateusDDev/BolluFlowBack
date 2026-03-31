using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class ProductComponent
{
    private ProductComponent() { }

    public long Id { get; private set; }
    public long ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public ProductComponentType ComponentType { get; private set; }
    public long? RecipeId { get; private set; }
    public Recipe? Recipe { get; private set; }
    public long? IngredientId { get; private set; }
    public Ingredient? Ingredient { get; private set; }
    public long? SupplyId { get; private set; }
    public Supply? Supply { get; private set; }
    public string Name { get; private set; } = null!;
    public string BaseUnit { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public decimal QuantityUsed { get; private set; }

    public static ProductComponent Create(long productId, ProductComponentType componentType, long sourceId, string name, string baseUnit, decimal unitPrice, decimal quantityUsed)
    {
        if (quantityUsed <= 0)
            throw new DomainException(ProductComponentErrors.InvalidQuantity);

        return new ProductComponent
        {
            ProductId = productId,
            ComponentType = componentType,
            RecipeId = componentType == ProductComponentType.Recipe ? sourceId : null,
            IngredientId = componentType == ProductComponentType.Ingredient ? sourceId : null,
            SupplyId = componentType == ProductComponentType.Supply ? sourceId : null,
            Name = name,
            BaseUnit = baseUnit,
            UnitPrice = unitPrice,
            QuantityUsed = quantityUsed
        };
    }

    public void Update(decimal quantityUsed, string name, string baseUnit, decimal unitPrice)
    {
        if (quantityUsed <= 0)
            throw new DomainException(ProductComponentErrors.InvalidQuantity);

        QuantityUsed = quantityUsed;
        Name = name;
        BaseUnit = baseUnit;
        UnitPrice = unitPrice;
    }
}
