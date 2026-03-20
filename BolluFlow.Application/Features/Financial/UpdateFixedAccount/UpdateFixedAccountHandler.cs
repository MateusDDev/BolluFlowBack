using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.UpdateFixedAccount;

public class UpdateFixedAccountHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<UpdateFixedAccountCommand, ResultOf<FixedAccountModel>>
{
    public async Task<ResultOf<FixedAccountModel>> Handle(UpdateFixedAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!FinancialMonth.TryParse(request.StartMonth, out var month))
                return ResultOf<FixedAccountModel>.BadRequest(FinancialErrors.InvalidMonthFormat);

            var account = await db.FixedAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == request.FixedAccountId, cancellationToken);
            if (account is null)
                return ResultOf<FixedAccountModel>.NotFound(FinancialErrors.FixedAccountNotFound);

            account.Update(request.Description, request.Category, request.Amount, month.Start, request.DefaultStatus);
            await db.SaveChangesAsync(cancellationToken);
            return ResultOf<FixedAccountModel>.Success(account.ToModel());
        }
        catch (DomainException ex)
        {
            return ResultOf<FixedAccountModel>.BadRequest(ex.Error);
        }
    }
}
