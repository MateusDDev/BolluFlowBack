using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class OrderModel 
{
    public long Id { get; set; }
    
    public int Number { get; set; }
    
    public CustomerModel Customer { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public string? Note { get; set; }
    
    public DateTime CreatedAt { get; set; }
}