namespace BolluFlow.Domain.Entities;

public class DeliveryAddress
{
    private DeliveryAddress() { }
    
    public long Id { get; set; }
    
    public string Street { get; set; }
    
    public int Number { get; set; }
    
    public string Neighborhood { get; set; }
    
    public long CustomerId { get; set; }
    
    public Customer Customer { get; set; }

    public static DeliveryAddress Create(string street, int number, string neighborhood)
    {
        return new DeliveryAddress
        {
            Street = street,
            Number = number,
            Neighborhood = neighborhood
        };
    }
}