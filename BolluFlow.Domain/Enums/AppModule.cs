namespace BolluFlow.Domain.Enums;

[Flags]
public enum AppModule
{
    None = 0,
    Orders = 1,
    Recipes = 2,
    Stock = 4,
    Finance = 8,
    All = Orders | Recipes | Stock | Finance
}
