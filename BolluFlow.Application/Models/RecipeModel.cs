namespace BolluFlow.Application.Models;

public class RecipeModel
{
    public long Id { get; private set; }

    public required string Name { get; set; }

    public required string Category { get; set; }

    public int? PreparationTime { get; set; }

    public decimal UnitCost { get; set; }

    public bool IsActive { get; set; }
}
