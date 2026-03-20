using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Finance.CreateTransaction;

public class CreateFinancialTransactionHandler(ApplicationDbContext db) : IRequestHandler<CreateFinancialTransactionCommand, ResultOf<FinancialTransactionDto>>
{
    public async Task<ResultOf<FinancialTransactionDto>> Handle(CreateFinancialTransactionCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return ResultOf<FinancialTransactionDto>.BadRequest(FinanceErrors.InvalidAmount);
        if (!FinanceMonth.TryParseDate(request.EventDate, out var eventDate))
            return ResultOf<FinancialTransactionDto>.BadRequest(FinanceErrors.InvalidDate);

        FinancialTransactionType type;
        switch (request.Type.ToLowerInvariant())
        {
            case "income":
                type = FinancialTransactionType.Income;
                break;
            case "expense":
                type = FinancialTransactionType.Expense;
                break;
            default:
                return ResultOf<FinancialTransactionDto>.BadRequest(new("FINANCE_INVALID_TYPE", "Type must be income or expense."));
        }

        var label = type == FinancialTransactionType.Income ? "Manual income" : "Manual expense";
        var transaction = FinancialTransaction.Create(
            type,
            FinancialTransactionSourceType.Manual,
            string.IsNullOrWhiteSpace(request.Description) ? label : request.Description,
            request.Amount,
            eventDate,
            FinancialSourceEntityType.Manual,
            null,
            false,
            request.Notes,
            null);

        db.FinancialTransactions.Add(transaction);
        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<FinancialTransactionDto>.Success(transaction.ToDto());
    }
}
