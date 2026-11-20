using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Services;

public static class AppModuleNames
{
    private static readonly (AppModule Module, string Name)[] Modules =
    [
        (AppModule.Orders, "orders"),
        (AppModule.Recipes, "recipes"),
        (AppModule.Stock, "stock"),
        (AppModule.Finance, "finance")
    ];

    public static List<string> ToNames(AppModule modules)
    {
        return Modules
            .Where(x => modules.HasFlag(x.Module))
            .Select(x => x.Name)
            .ToList();
    }

    public static bool TryParse(IEnumerable<string>? names, out AppModule modules)
    {
        modules = AppModule.None;

        if (names is null)
            return true;

        foreach (var name in names)
        {
            var match = Modules.FirstOrDefault(x => x.Name.Equals(name?.Trim(), StringComparison.OrdinalIgnoreCase));

            if (match.Module == AppModule.None)
                return false;

            modules |= match.Module;
        }

        return true;
    }
}
