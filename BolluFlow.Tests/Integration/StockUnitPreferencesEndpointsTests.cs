using System.Net;
using System.Net.Http.Json;
using BolluFlow.Application.Features.Preferences.StockUnit.SaveStockUnitPreference;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BolluFlow.Tests.Integration;

public class StockUnitPreferencesEndpointsTests(DashboardWebApplicationFactory factory) : IClassFixture<DashboardWebApplicationFactory>
{
    [Fact]
    public async Task ShouldSaveAndReadStockUnitPreferenceUsingEnumStrings()
    {
        await ResetDatabaseAsync();
        var ingredientId = await SeedIngredientAsync();

        var client = factory.CreateClient();
        var body = new SaveStockUnitPreferenceBody(StockItemType.Ingredient, ingredientId, null);

        var saveResponse = await client.PostAsJsonAsync("/preferences/stock-unit", body);
        var saveJson = await saveResponse.Content.ReadAsStringAsync();
        var savedPreference = await saveResponse.Content.ReadFromJsonAsync<StockUnitPreferenceModel>();
        var getJson = await client.GetStringAsync($"/preferences/stock-unit?itemType=Ingredient&itemId={ingredientId}");

        Assert.Equal(HttpStatusCode.Created, saveResponse.StatusCode);
        Assert.NotNull(savedPreference);
        Assert.Equal(StockItemType.Ingredient, savedPreference!.ItemType);
        Assert.Contains("\"itemType\":\"Ingredient\"", saveJson);
        Assert.Contains("\"itemType\":\"Ingredient\"", getJson);
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.UserStockUnitPreferences.RemoveRange(db.UserStockUnitPreferences);
        db.IngredientConversions.RemoveRange(db.IngredientConversions);
        db.SupplyConversions.RemoveRange(db.SupplyConversions);
        db.Ingredients.RemoveRange(db.Ingredients);
        db.Supplies.RemoveRange(db.Supplies);

        await db.SaveChangesAsync();
    }

    private async Task<long> SeedIngredientAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var ingredient = Ingredient.Create(1, "Acucar", BaseUnit.Gram, 5, 1000, 200);
        db.Ingredients.Add(ingredient);
        await db.SaveChangesAsync();

        return ingredient.Id;
    }
}
