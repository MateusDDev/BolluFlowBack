using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.DeleteFixedAccountRange;

public class DeleteFixedAccountRangeHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<DeleteFixedAccountRangeCommand, ResultOf<FixedAccountModel>>
{
    public async Task<ResultOf<FixedAccountModel>> Handle(DeleteFixedAccountRangeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            DateOnly? month = null;
            if (!string.IsNullOrWhiteSpace(request.Month))
            {
                if (!FinancialMonth.TryParse(request.Month, out var parsed))
                    return ResultOf<FixedAccountModel>.BadRequest(FinancialErrors.InvalidMonthFormat);

                month = parsed.Start;
            }

            var account = await db.FixedAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == request.FixedAccountId, cancellationToken);
            if (account is null)
                return ResultOf<FixedAccountModel>.NotFound(FinancialErrors.FixedAccountNotFound);

            account.ApplyDeleteRange(request.Mode, month);
            await db.SaveChangesAsync(cancellationToken);
            return ResultOf<FixedAccountModel>.Success(account.ToModel());
        }
        catch (DomainException ex)
        {
            return ResultOf<FixedAccountModel>.BadRequest(ex.Error);
        }
    }
}
