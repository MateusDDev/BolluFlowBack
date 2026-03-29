using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class RecipeErrors
{
    public static readonly DomainError FinalPriceInvalid =
        new("RECIPE_FINAL_PRICE_INVALID", "Preço final da receita inválido");

    public static readonly DomainError QuantityInvalid =
        new("RECIPE_QUANTITY_INVALID", "Quantidade da receita deve ser maior que zero");

    public static readonly DomainError MinimumStockQuantityInvalid =
        new("RECIPE_MINIMUM_STOCK_QUANTITY_INVALID", "Estoque mínimo da receita inválido");

    public static DomainError StockInsufficient(string recipeName) =>
        new("RECIPE_STOCK_INSUFFICIENT", $"Estoque insuficiente para a receita {recipeName}");
}
