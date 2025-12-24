namespace BolluFlow.Domain.Entities;

public class RecipeIngredient
{
    private RecipeIngredient() { }
    
    public long RecipeId { get; set; }
    
    public Recipe Recipe { get; set; } = null!;

    public long IngredientId { get; set; }
    
    public Ingredient Ingredient { get; set; } = null!;

    public int QuantityUsed { get; set; }

    public static RecipeIngredient Create(
        Recipe recipe, 
        Ingredient ingredient,
        int quantityUsed)
    {
        return new RecipeIngredient
        {
            Recipe = recipe,
            Ingredient = ingredient,
            QuantityUsed = quantityUsed
        };
    }
}