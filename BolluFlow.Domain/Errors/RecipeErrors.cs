using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class RecipeErrors
{
    public static readonly DomainError FinalPriceInvalid = new("RECIPE_FINAL_PRICE_INVALID", "Preço final da receita inválido");
    public static readonly DomainError BaseUnitRequired = new("RECIPE_BASE_UNIT_REQUIRED", "Unidade base da receita é obrigatória");
    public static readonly DomainError StockInvalid = new("RECIPE_STOCK_INVALID", "Estoque da receita não pode ser negativo");
    public static readonly DomainError ConversionNotFound = new("RECIPE_CONVERSION_NOT_FOUND", "Conversão da receita não encontrada");
    public static readonly DomainError ConversionNameRequired = new("RECIPE_CONVERSION_NAME_REQUIRED", "Nome da conversão da receita é obrigatório");
    public static readonly DomainError ConversionQuantityInvalid = new("RECIPE_CONVERSION_QUANTITY_INVALID", "Quantidade da conversão da receita deve ser maior que zero");
    public static readonly DomainError ProductionQuantityInvalid = new("RECIPE_PRODUCTION_QUANTITY_INVALID", "Quantidade produzida deve ser maior que zero");
}
