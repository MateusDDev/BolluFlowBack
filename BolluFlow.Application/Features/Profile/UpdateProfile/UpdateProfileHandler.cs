using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using BolluFlow.Domain.ValueObjects;
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

        if (body.DeliveryTimeMinutes.Value is < 1 or > 1440)
            return ResultOf<ProfileModel>.BadRequest(new DomainError("PROFILE_DELIVERY_TIME_INVALID", "O tempo de entrega deve estar entre 1 minuto e 24 horas"));

        if (body.PixKey.Value?.Trim().Length > 140)
            return ResultOf<ProfileModel>.BadRequest(new DomainError("PROFILE_PIX_KEY_INVALID", "A chave Pix deve ter no máximo 140 caracteres"));

        var storeAddressBody = body.StoreAddress.Value;

        if (storeAddressBody is not null)
        {
            if (string.IsNullOrWhiteSpace(storeAddressBody.Street) || string.IsNullOrWhiteSpace(storeAddressBody.Neighborhood))
                return ResultOf<ProfileModel>.BadRequest(new DomainError("PROFILE_STORE_ADDRESS_INVALID", "Informe rua e bairro do endereço da doceria"));

            if (storeAddressBody.Number is null or <= 0)
                return ResultOf<ProfileModel>.BadRequest(new DomainError("PROFILE_STORE_ADDRESS_NUMBER_INVALID", "Informe o número do endereço da doceria"));
        }

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

        if (body.EnabledModules is not null)
        {
            if (!AppModuleNames.TryParse(body.EnabledModules, out var modules))
                return ResultOf<ProfileModel>.BadRequest(new DomainError("PROFILE_MODULES_INVALID", "enabledModules contém um módulo inválido"));

            user.SetEnabledModules(modules);
        }

        if (body.AllowNegativeStock.HasValue)
            user.SetAllowNegativeStock(body.AllowNegativeStock.Value);

        if (body.DeliveryTimeMinutes.IsSet)
            user.SetDeliveryTimeMinutes(body.DeliveryTimeMinutes.Value);

        if (body.StoreAddress.IsSet)
        {
            user.SetStoreAddress(storeAddressBody is null
                ? null
                : new StoreAddress(
                    storeAddressBody.Street!.Trim(),
                    storeAddressBody.Number!.Value,
                    storeAddressBody.Neighborhood!.Trim(),
                    string.IsNullOrWhiteSpace(storeAddressBody.Complement) ? null : storeAddressBody.Complement.Trim()));
        }

        if (body.PixKey.IsSet)
            user.SetPixKey(body.PixKey.Value);

        if (body.Whatsapp is not null)
            user.SetWhatsapp(body.Whatsapp);

        var whatsappMessageTemplate = body.WhatsappMessageTemplate is null
            ? user.WhatsappMessageTemplate
            : string.IsNullOrWhiteSpace(body.WhatsappMessageTemplate) ? null : body.WhatsappMessageTemplate;

        user.UpdateProfile(
            trimmedName,
            whatsappMessageTemplate,
            body.IndirectCostRate);

        await db.SaveChangesAsync(cancellationToken);

        var model = ProfileModel.From(user);

        return ResultOf<ProfileModel>.Success(model);
    }
}
