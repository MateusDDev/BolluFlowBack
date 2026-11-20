using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Services;

public static class ProductComponentCostService
{
    public static decimal CalculateComponentCost(ProductComponent component)
    {
        return component.CurrentUnitCost * component.QuantityUsed;
    }
}
