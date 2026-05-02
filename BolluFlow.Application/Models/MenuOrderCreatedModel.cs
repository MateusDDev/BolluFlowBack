namespace BolluFlow.Application.Models;

public class MenuOrderCreatedModel
{
    public long OrderId { get; set; }
    public int OrderNumber { get; set; }
    public string Message { get; set; } = null!;
}
