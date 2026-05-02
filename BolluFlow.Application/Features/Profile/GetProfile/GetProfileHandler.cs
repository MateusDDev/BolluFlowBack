using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Profile.GetProfile;

public class GetProfileHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetProfileQuery, ResultOf<ProfileModel>>
{
    public async Task<ResultOf<ProfileModel>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentUser.Id, cancellationToken);

        if (user is null)
            return ResultOf<ProfileModel>.NotFound(CommonErrors.UserNotFound);

        var model = new ProfileModel
        {
            Name = user.Name,
            Slug = user.Slug,
            WhatsappMessageTemplate = user.WhatsappMessageTemplate,
            IndirectCostRate = user.IndirectCostRate,
            SkipStockCheck = user.SkipStockCheck
        };

        return ResultOf<ProfileModel>.Success(model);
    }
}
