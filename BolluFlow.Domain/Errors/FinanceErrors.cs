using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public static class FinanceErrors
{
    public static readonly DomainError InvalidMonth = new("FINANCE_INVALID_MONTH", "Month must use the YYYY-MM format.");
    public static readonly DomainError InvalidDate = new("FINANCE_INVALID_DATE", "Date must use a valid calendar date.");
    public static readonly DomainError InvalidAmount = new("FINANCE_INVALID_AMOUNT", "Amount must be greater than zero.");
    public static readonly DomainError TransactionNotFound = new("FINANCE_TRANSACTION_NOT_FOUND", "Financial transaction was not found.");
    public static readonly DomainError FixedCostNotFound = new("FIXED_COST_NOT_FOUND", "Fixed cost was not found.");
    public static readonly DomainError PurchaseNotFound = new("PURCHASE_NOT_FOUND", "Purchase was not found.");
    public static readonly DomainError SaleTransactionDeleteBlocked = new("SALE_TRANSACTION_DELETE_BLOCKED", "Sale transactions cannot be deleted because they are linked to delivered orders.");
    public static readonly DomainError FixedCostTransactionDeleteBlocked = new("FIXED_COST_TRANSACTION_DELETE_BLOCKED", "Recurring fixed cost transactions must be managed from the fixed cost endpoint.");
    public static readonly DomainError InvalidDeleteMode = new("FIXED_COST_INVALID_DELETE_MODE", "Delete mode must be until-this-month, from-this-month-forward, or permanent.");
    public static readonly DomainError InvalidItemType = new("PURCHASE_INVALID_ITEM_TYPE", "Only ingredient and supply purchases are supported.");
}
