namespace BolluFlow.Application.Models;

public class RecipeIngredientModel
{
    public long RecipeId { get; set; }

    public long IngredientId { get; set; }

    public int QuantityUsed { get; set; }
}