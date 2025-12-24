namespace BolluFlow.Application.Models;

public class RecipeModel
{
    public long Id { get; private set; }
    
    public required string Name { get; set; }
    
    public required string Category { get; set; }
    
    public int? PreparationTime { get; set; }
    
    public decimal IndirectCostPercentage { get; set; }
    
    public decimal MarkupPercentage { get; set; }
    
    public decimal FinalPrice { get; set; }
    
    public decimal? DiscountPercentage { get; set; }
    
    public bool IsActive { get; set; }

    public List<RecipeIngredientModel> RecipeIngredients { get; set; } = [];
}