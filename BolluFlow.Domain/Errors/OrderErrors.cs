using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class OrderErrors
{
    public static readonly DomainError RecipeInactive =
        new("RECIPE_INACTIVE", "A receita precisa estar ativa para ser adicionada ao pedido");
}