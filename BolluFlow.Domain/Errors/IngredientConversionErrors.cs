using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class IngredientConversionErrors
{
    public static readonly DomainError NameRequired =
        new("INGREDIENT_CONVERSION_NAME_REQUIRED", "O campo \"Nome\" é obrigatório");
    
    public static readonly DomainError QuantityInvalid =
        new("INGREDIENT_CONVERSION_QUANTITY_INVALID", "Quantidade inválida");
}