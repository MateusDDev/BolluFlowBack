namespace BolluFlow.Domain.Entities;

public class RecipeProduction
{
    private RecipeProduction() { }

    public long Id { get; private set; }

    public long RecipeId { get; private set; }

    public Recipe Recipe { get; private set; } = null!;

    public decimal Quantity { get; private set; }

    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static RecipeProduction Create(long recipeId, decimal quantity, string? notes)
    {
        return new RecipeProduction
        {
            RecipeId = recipeId,
            Quantity = quantity,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }
}
