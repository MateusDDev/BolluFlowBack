using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public static class FinancialErrors
{
    public static readonly DomainError DescriptionRequired = new("FINANCIAL_DESCRIPTION_REQUIRED", "Descrição do lançamento é obrigatória");
    public static readonly DomainError AmountMustBePositive = new("FINANCIAL_AMOUNT_POSITIVE_REQUIRED", "Valor do lançamento deve ser maior que zero");
    public static readonly DomainError EntryNotFound = new("FINANCIAL_ENTRY_NOT_FOUND", "Lançamento financeiro não encontrado");
    public static readonly DomainError CannotDeleteAutomaticEntry = new("FINANCIAL_AUTO_DELETE_FORBIDDEN", "Lançamentos automáticos não podem ser excluídos manualmente");
    public static readonly DomainError AutomaticEntryCannotBeEdited = new("FINANCIAL_AUTO_EDIT_FORBIDDEN", "Lançamentos automáticos não podem ser editados manualmente");
    public static readonly DomainError FinancialEntryTypeImmutable = new("FINANCIAL_ENTRY_TYPE_IMMUTABLE", "Tipo do lançamento não pode ser alterado");
    public static readonly DomainError FixedAccountDescriptionRequired = new("FIXED_ACCOUNT_DESCRIPTION_REQUIRED", "Descrição da conta fixa é obrigatória");
    public static readonly DomainError FixedAccountAmountMustBePositive = new("FIXED_ACCOUNT_AMOUNT_POSITIVE_REQUIRED", "Valor padrão da conta fixa deve ser maior que zero");
    public static readonly DomainError DueDayInvalid = new("FIXED_ACCOUNT_DUE_DAY_INVALID", "Dia de vencimento deve estar entre 1 e 28");
    public static readonly DomainError InvalidPagination = new("FINANCIAL_INVALID_PAGINATION", "Parâmetros de paginação inválidos");
    public static readonly DomainError InvalidMonthFormat = new("FINANCIAL_INVALID_MONTH", "Mês inválido. Use o formato YYYY-MM");
    public static readonly DomainError ReferenceRequiredForAutomaticEntry = new("FINANCIAL_REFERENCE_REQUIRED", "Lançamentos automáticos precisam de referência de origem");
    public static readonly DomainError InvalidAutomaticSource = new("FINANCIAL_INVALID_SOURCE", "Fonte automática inválida para o lançamento");
    public static readonly DomainError StockPurchaseQuantityMustBePositive = new("FINANCIAL_STOCK_PURCHASE_QUANTITY_INVALID", "Quantidade da compra de estoque deve ser maior que zero");
    public static readonly DomainError StockPurchaseUnitCostMustBePositive = new("FINANCIAL_STOCK_PURCHASE_UNIT_COST_INVALID", "Custo unitário da compra de estoque deve ser maior que zero");
}
