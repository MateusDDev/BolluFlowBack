using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public static class SupplyErrors
{
    public static readonly DomainError NameRequired = new("SUPPLY_NAME_REQUIRED", "Nome do insumo é obrigatório");
    public static readonly DomainError UnitCostInvalid = new("SUPPLY_UNIT_COST_INVALID", "Custo unitário do insumo inválido");
    public static readonly DomainError StockQuantityInvalid = new("SUPPLY_STOCK_QUANTITY_INVALID", "Quantidade de estoque do insumo inválida");
    public static readonly DomainError MinimumStockQuantityInvalid = new("SUPPLY_MINIMUM_STOCK_INVALID", "Estoque mínimo do insumo inválido");
    public static DomainError StockInsufficient(string name) => new("SUPPLY_STOCK_INSUFFICIENT", $"Estoque insuficiente do insumo {name}");
}
