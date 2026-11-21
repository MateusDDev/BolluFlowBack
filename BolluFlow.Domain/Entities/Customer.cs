namespace BolluFlow.Domain.Entities;

public class Customer
{
    protected Customer() { }
    
    public long Id { get; private set; }
    
    public long UserId { get; private set; }
    
    public User User { get; set; }
    
    public string Name { get; set; }
    
    public string Whatsapp { get; set; }
    
    public string? Preferences { get; set; }
    
    public DateTime? Birthday { get; set; }
    
    public DeliveryAddress? DeliveryAddress { get; set; }

    public static Customer Create(
        long userId,
        string name,
        string whatsapp,
        DeliveryAddress? deliveryAddress,
        string? preferences = null,
        DateTime? birthday = null)
    {
        return new Customer
        {
            UserId = userId,
            Name = name,
            Whatsapp = whatsapp,
            DeliveryAddress = deliveryAddress,
            Preferences = preferences,
            Birthday = birthday
        };
    }

    public void Update(string name, DeliveryAddress? deliveryAddress, string? preferences = null, DateTime? birthday = null)
    {
        Name = name;
        Preferences = preferences;
        Birthday = birthday;

        if (deliveryAddress is null)
            return;

        if (DeliveryAddress is null)
        {
            DeliveryAddress = deliveryAddress;
            return;
        }

        DeliveryAddress.Update(deliveryAddress.Street, deliveryAddress.Number, deliveryAddress.Neighborhood, deliveryAddress.Complement);
    }
}
