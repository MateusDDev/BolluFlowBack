namespace BolluFlow.Application.Models;

public class IngredientModel
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string BaseUnit { get; set; }
    
    public decimal UnitCost { get; set; }
    
    public decimal StockQuantity { get; set; }
    
    public decimal? MinimumStockQuantity { get; set; }
}