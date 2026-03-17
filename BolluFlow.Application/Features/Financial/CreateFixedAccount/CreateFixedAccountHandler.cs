using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
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
            var account = FixedAccount.Create(user.Id, request.Description, request.Category, request.DefaultAmount, request.DueDay, request.IsActive);

            db.FixedAccounts.Add(account);
            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<FixedAccountModel>.Success(new FixedAccountModel
            {
                Id = account.Id,
                Description = account.Description,
                Category = account.Category,
                DefaultAmount = account.DefaultAmount,
                DueDay = account.DueDay,
                IsActive = account.IsActive
            });
        }
        catch (DomainException ex)
        {
            return ResultOf<FixedAccountModel>.BadRequest(ex.Error);
        }
    }
}
