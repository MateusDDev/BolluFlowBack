namespace BolluFlow.Application.Models;

public class ProductComponentCatalogItemModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string BaseUnit { get; set; } = null!;
    public string SourceType { get; set; } = null!;
}
