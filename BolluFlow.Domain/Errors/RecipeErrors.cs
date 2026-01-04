using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class RecipeErrors
{
    public static readonly DomainError InvalidDiscount =
        new("DISCOUNT_INVALID", "O desconto deve estar entre 0 e 100 por cento");
}