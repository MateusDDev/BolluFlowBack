using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Financial.CreateFixedAccount;

public class CreateFixedAccountHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateFixedAccountCommand, ResultOf<FixedAccountModel>>
{
    public async Task<ResultOf<FixedAccountModel>> Handle(CreateFixedAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!FinancialMonth.TryParse(request.StartMonth, out var month))
                return ResultOf<FixedAccountModel>.BadRequest(FinancialErrors.InvalidMonthFormat);

            var account = FixedAccount.Create(user.Id, request.Description, request.Category, request.Amount, month.Start, request.DefaultStatus);
            db.FixedAccounts.Add(account);
            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<FixedAccountModel>.Success(account.ToModel());
        }
        catch (DomainException ex)
        {
            return ResultOf<FixedAccountModel>.BadRequest(ex.Error);
        }
    }
}
