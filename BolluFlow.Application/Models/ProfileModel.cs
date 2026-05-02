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

    public bool SkipStockCheck { get; set; }
}
