using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class RecipeErrors
{
    public static readonly DomainError FinalPriceInvalid =
        new("RECIPE_FINAL_PRICE_INVALID", "Preço final da receita inválido");
}
