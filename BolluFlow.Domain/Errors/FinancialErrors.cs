using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public static class FinancialErrors
{
    public static readonly DomainError DescriptionRequired = new("FINANCIAL_DESCRIPTION_REQUIRED", "Descrição do lançamento é obrigatória");
    public static readonly DomainError AmountMustBePositive = new("FINANCIAL_AMOUNT_POSITIVE_REQUIRED", "Valor do lançamento deve ser maior que zero");
    public static readonly DomainError EntryNotFound = new("FINANCIAL_ENTRY_NOT_FOUND", "Lançamento financeiro não encontrado");
    public static readonly DomainError CannotDeleteAutomaticSaleEntry = new("FINANCIAL_AUTO_SALE_DELETE_FORBIDDEN", "Lançamentos automáticos de venda não podem ser excluídos manualmente");
    public static readonly DomainError FinancialEntryTypeImmutable = new("FINANCIAL_ENTRY_TYPE_IMMUTABLE", "Tipo do lançamento não pode ser alterado");
    public static readonly DomainError FixedAccountNotFound = new("FIXED_ACCOUNT_NOT_FOUND", "Conta fixa não encontrada");
    public static readonly DomainError FixedAccountDescriptionRequired = new("FIXED_ACCOUNT_DESCRIPTION_REQUIRED", "Descrição da conta fixa é obrigatória");
    public static readonly DomainError FixedAccountAmountMustBePositive = new("FIXED_ACCOUNT_AMOUNT_POSITIVE_REQUIRED", "Valor padrão da conta fixa deve ser maior que zero");
    public static readonly DomainError DueDayInvalid = new("FIXED_ACCOUNT_DUE_DAY_INVALID", "Dia de vencimento deve estar entre 1 e 28");
    public static readonly DomainError InvalidPagination = new("FINANCIAL_INVALID_PAGINATION", "Parâmetros de paginação inválidos");
    public static readonly DomainError InvalidMonthFormat = new("FINANCIAL_INVALID_MONTH", "Mês inválido. Use o formato YYYY-MM");
}
