namespace BolluFlow.Application.Models;

public class IngredientModel
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Unit { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal StockQuantity { get; set; }
    
    public decimal? MinimumStockQuantity { get; set; }
}