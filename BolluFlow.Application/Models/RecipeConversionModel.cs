using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Models;

public class RecipeConversionModel
{
    public long Id { get; set; }

    public long RecipeId { get; set; }

    public required string Name { get; set; }

    public decimal QuantityInBaseUnit { get; set; }

    public static RecipeConversionModel From(RecipeConversion conversion)
    {
        return new RecipeConversionModel
        {
            Id = conversion.Id,
            RecipeId = conversion.RecipeId,
            Name = conversion.Name,
            QuantityInBaseUnit = conversion.QuantityInBaseUnit
        };
    }
}
