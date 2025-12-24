namespace BolluFlow.Application.Models;

public class DeliveryAddressModel
{
    public long Id { get; set; }
    
    public string Street { get; set; }
    
    public int Number { get; set; }
    
    public string Neighborhood { get; set; }
}