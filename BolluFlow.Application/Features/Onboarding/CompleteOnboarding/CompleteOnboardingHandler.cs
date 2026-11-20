using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Onboarding.CompleteOnboarding;

public class CompleteOnboardingHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<CompleteOnboardingCommand, ResultOf<UserModel>>
{
    public async Task<ResultOf<UserModel>> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        if (!AppModuleNames.TryParse(request.EnabledModules, out var modules))
            return ResultOf<UserModel>.BadRequest(new DomainError("ONBOARDING_MODULES_INVALID", "enabledModules contém um módulo inválido"));

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == currentUser.Id, cancellationToken);

        if (user is null)
            return ResultOf<UserModel>.NotFound(CommonErrors.UserNotFound);

        user.CompleteOnboarding(modules);

        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<UserModel>.Success(UserModel.From(user));
    }
}
