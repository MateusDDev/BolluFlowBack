using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Profile.UpdateProfile;

public record UpdateProfileCommand(UpdateProfileCommandBody Body) : ICommand<ResultOf<ProfileModel>>;

public record UpdateProfileCommandBody(
    string? Name,
    string? Slug,
    /// <summary>
    /// Variáveis suportadas no template:
    /// {customerName}, {orderNumber}, {items}, {total}, {paymentMethod}, {deliveryDate}, {address}
    /// </summary>
    string? WhatsappMessageTemplate,
    decimal? IndirectCostRate,
    string? Whatsapp = null,
    List<string>? EnabledModules = null,
    bool? AllowNegativeStock = null,
    OptionalField<int?> DeliveryTimeMinutes = default,
    OptionalField<string?> PixKey = default,
    OptionalField<UpdateProfileStoreAddress?> StoreAddress = default);

public record UpdateProfileStoreAddress(string? Street, int? Number, string? Neighborhood, string? Complement);
