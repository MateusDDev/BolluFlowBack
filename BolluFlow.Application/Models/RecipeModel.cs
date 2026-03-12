namespace BolluFlow.Application.Models;

public class RecipeModel
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public required string Category { get; set; }

    public int? PreparationTime { get; set; }

    public decimal UnitCost { get; set; }

    public decimal FinalPrice { get; set; }

    public bool IsActive { get; set; }
}
