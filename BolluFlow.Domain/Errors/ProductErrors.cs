using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public static class ProductErrors
{
    public static readonly DomainError NameRequired = new("PRODUCT_NAME_REQUIRED", "Nome do produto é obrigatório");
    public static readonly DomainError StockQuantityInvalid = new("PRODUCT_STOCK_QUANTITY_INVALID", "Quantidade de estoque do produto inválida");
    public static readonly DomainError MinimumStockQuantityInvalid = new("PRODUCT_MINIMUM_STOCK_INVALID", "Estoque mínimo do produto inválido");
    public static readonly DomainError FinalPriceInvalid = new("PRODUCT_FINAL_PRICE_INVALID", "Preço final do produto inválido");
    public static readonly DomainError InvalidDiscount = new("PRODUCT_INVALID_DISCOUNT", "Desconto do produto inválido");
    public static DomainError StockInsufficient(string name) => new("PRODUCT_STOCK_INSUFFICIENT", $"Estoque insuficiente do produto {name}");
    public static readonly DomainError ProductInactive = new("PRODUCT_INACTIVE", "Produto inativo não pode ser adicionado ao pedido");
}
