using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Profile.UpdateProfile;

public class UpdateProfileHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<UpdateProfileCommand, ResultOf<ProfileModel>>
{
    public async Task<ResultOf<ProfileModel>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var body = request.Body;
        var trimmedName = body.Name?.Trim();

        if (trimmedName is not null && trimmedName.Length < 2)
            return ResultOf<ProfileModel>.BadRequest(new DomainError("PROFILE_NAME_INVALID", "name deve ter no mínimo 2 caracteres"));

        if (body.IndirectCostRate is < 0 or > 100)
            return ResultOf<ProfileModel>.BadRequest(new DomainError("PROFILE_INDIRECT_COST_RATE_INVALID", "indirectCostRate deve estar entre 0 e 100"));

        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Id == currentUser.Id, cancellationToken);

        if (user is null)
            return ResultOf<ProfileModel>.NotFound(CommonErrors.UserNotFound);

        if (body.Slug is not null)
        {
            if (string.IsNullOrWhiteSpace(body.Slug))
            {
                user.SetSlug(null);
            }
            else
            {
                var normalizedSlug = SlugService.Generate(body.Slug);

                if (normalizedSlug is null)
                    return ResultOf<ProfileModel>.BadRequest(new DomainError("PROFILE_SLUG_INVALID", "slug inválido"));

                var slugIsTaken = await db.Users
                    .AnyAsync(x => x.Id != user.Id && x.Slug == normalizedSlug, cancellationToken);

                if (slugIsTaken)
                    return ResultOf<ProfileModel>.Conflict(new DomainError("PROFILE_SLUG_ALREADY_EXISTS", "slug já está em uso"));

                user.SetSlug(normalizedSlug);
            }
        }

        user.UpdateProfile(
            trimmedName,
            body.WhatsappMessageTemplate,
            body.IndirectCostRate,
            body.SkipStockCheck);

        await db.SaveChangesAsync(cancellationToken);

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
