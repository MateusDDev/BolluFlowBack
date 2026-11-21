namespace BolluFlow.Application.Models;

public class MenuStoreModel
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Whatsapp { get; set; }
    public int? DeliveryTimeMinutes { get; set; }
    public string? PixKey { get; set; }
    public StoreAddressModel? StoreAddress { get; set; }
}
