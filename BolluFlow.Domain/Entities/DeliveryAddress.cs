namespace BolluFlow.Domain.Entities;

public class DeliveryAddress
{
    public long Id { get; set; }
    
    public string Street { get; set; }
    
    public string Number { get; set; }
    
    public string Neighborhood { get; set; }
    
    public long CustomerId { get; set; }
    
    public Customer Customer { get; set; }
}