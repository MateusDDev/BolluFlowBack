using BolluFlow.Application.Services;
using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Models;

public class ProfileModel
{
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }

    /// <summary>
    /// Variáveis suportadas no template:
    /// {customerName}, {orderNumber}, {items}, {total}, {paymentMethod}, {deliveryDate}, {address}
    /// </summary>
    public string? WhatsappMessageTemplate { get; set; }

    public decimal? IndirectCostRate { get; set; }

    public string? Whatsapp { get; set; }

    public List<string> EnabledModules { get; set; } = [];

    public bool AllowNegativeStock { get; set; }

    public static ProfileModel From(User user)
    {
        return new ProfileModel
        {
            Name = user.Name,
            Slug = user.Slug,
            WhatsappMessageTemplate = user.WhatsappMessageTemplate,
            IndirectCostRate = user.IndirectCostRate,
            Whatsapp = user.Whatsapp,
            EnabledModules = AppModuleNames.ToNames(user.EnabledModules),
            AllowNegativeStock = user.AllowNegativeStock
        };
    }
}
