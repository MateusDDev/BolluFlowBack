using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Services;

public static class ProductComponentCostService
{
    public static decimal CalculateComponentCost(ProductComponent component)
    {
        var unitCost = component.ComponentType == ProductComponentType.Recipe
            ? component.Recipe?.CostPerBaseUnit ?? 0m
            : component.UnitPrice;

        return unitCost * component.QuantityUsed;
    }
}
