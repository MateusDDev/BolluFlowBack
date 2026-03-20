using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public static class FinancialErrors
{
    public static readonly DomainError DescriptionRequired = new("FINANCIAL_DESCRIPTION_REQUIRED", "Descrição do lançamento é obrigatória");
    public static readonly DomainError AmountMustBePositive = new("FINANCIAL_AMOUNT_POSITIVE_REQUIRED", "Valor do lançamento deve ser maior que zero");
    public static readonly DomainError EntryNotFound = new("FINANCIAL_ENTRY_NOT_FOUND", "Lançamento financeiro não encontrado");
    public static readonly DomainError EntryAlreadyExcluded = new("FINANCIAL_ENTRY_ALREADY_EXCLUDED", "Lançamento financeiro já está excluído");
    public static readonly DomainError AutomaticEntryCannotBeEdited = new("FINANCIAL_AUTO_EDIT_FORBIDDEN", "Lançamentos automáticos não podem ser editados manualmente");
    public static readonly DomainError FinancialEntryTypeImmutable = new("FINANCIAL_ENTRY_TYPE_IMMUTABLE", "Tipo do lançamento não pode ser alterado");
    public static readonly DomainError FixedAccountDescriptionRequired = new("FIXED_ACCOUNT_DESCRIPTION_REQUIRED", "Descrição da conta fixa é obrigatória");
    public static readonly DomainError FixedAccountAmountMustBePositive = new("FIXED_ACCOUNT_AMOUNT_POSITIVE_REQUIRED", "Valor da conta fixa deve ser maior que zero");
    public static readonly DomainError FixedAccountNotFound = new("FIXED_ACCOUNT_NOT_FOUND", "Conta fixa não encontrada");
    public static readonly DomainError InvalidPagination = new("FINANCIAL_INVALID_PAGINATION", "Parâmetros de paginação inválidos");
    public static readonly DomainError InvalidMonthFormat = new("FINANCIAL_INVALID_MONTH", "Mês inválido. Use o formato YYYY-MM");
    public static readonly DomainError ReferenceRequiredForAutomaticEntry = new("FINANCIAL_REFERENCE_REQUIRED", "Lançamentos automáticos precisam de referência de origem");
    public static readonly DomainError InvalidAutomaticSource = new("FINANCIAL_INVALID_SOURCE", "Fonte automática inválida para o lançamento");
    public static readonly DomainError InvalidStatusTransition = new("FINANCIAL_INVALID_STATUS", "Status informado não é permitido para esta operação");
    public static readonly DomainError StockPurchaseQuantityMustBePositive = new("FINANCIAL_STOCK_PURCHASE_QUANTITY_INVALID", "Quantidade da compra de estoque deve ser maior que zero");
    public static readonly DomainError StockPurchaseUnitCostMustBePositive = new("FINANCIAL_STOCK_PURCHASE_UNIT_COST_INVALID", "Custo unitário da compra de estoque deve ser maior que zero");
    public static readonly DomainError ExclusionReasonRequired = new("FINANCIAL_EXCLUSION_REASON_REQUIRED", "Motivo da exclusão é obrigatório");
    public static readonly DomainError InvalidFilter = new("FINANCIAL_INVALID_FILTER", "Filtro informado é inválido");
}
