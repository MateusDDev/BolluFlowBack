namespace BolluFlow.Domain.Entities;

public class Customer
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Whatsapp { get; set; }
    
    public string? Preferences { get; set; }
    
    public DateTime? Birthday { get; set; }
    
    public DeliveryAddress DeliveryAddress { get; set; }
}