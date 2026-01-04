using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public static class IngredientErrors
{
    public static readonly DomainError NameRequired =
        new("INGREDIENT_NAME_REQUIRED", "O campo \"Nome\" é obrigatório");

    public static readonly DomainError UnitRequired =
        new("INGREDIENT_UNIT_REQUIRED", "O campo \"Unidade\" é obrigatório");

    public static readonly DomainError UnitPriceInvalid =
        new("INGREDIENT_UNIT_PRICE_INVALID", "Preço unitário inválido");

    public static DomainError StockInsufficient(string name) =>
        new("INGREDIENT_STOCK_INSUFFICIENT", $"Estoque insuficiente para {name}");

    public static readonly DomainError QuantityInvalid =
        new("INGREDIENT_QUANTITY_INVALID", "Quantidade inválida");
}
