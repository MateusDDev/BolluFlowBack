using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Profile.UpdateProfile;

public record UpdateProfileCommand(UpdateProfileCommandBody Body) : ICommand<ResultOf<ProfileModel>>;

public record UpdateProfileCommandBody(
    string? Name,
    /// <summary>
    /// Variáveis suportadas no template:
    /// {customerName}, {orderNumber}, {items}, {total}, {paymentMethod}, {deliveryDate}, {address}
    /// </summary>
    string? WhatsappMessageTemplate,
    decimal? IndirectCostRate,
    bool? SkipStockCheck);
