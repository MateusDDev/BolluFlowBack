namespace BolluFlow.Application.Models;

public class MenuProductModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal FinalPrice { get; set; }
    public string? Category { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
}
