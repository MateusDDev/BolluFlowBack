namespace BolluFlow.Domain.Entities;

public class RecipeIngredient
{
    private RecipeIngredient() { }
    
    public long RecipeId { get; set; }
    
    public Recipe Recipe { get; set; } = null!;

    public long IngredientId { get; set; }
    
    public Ingredient Ingredient { get; set; } = null!;

    public decimal QuantityUsed { get; set; }

    public static RecipeIngredient Create(
        long recipeId, 
        long ingredientId,
        decimal quantityUsed)
    {
        return new RecipeIngredient
        {
            RecipeId = recipeId,
            IngredientId = ingredientId,
            QuantityUsed = quantityUsed
        };
    }
}