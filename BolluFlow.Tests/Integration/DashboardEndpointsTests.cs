using System.Net;
using System.Net.Http.Json;
using BolluFlow.Application.Features.Dashboard.GetInventoryValueByType;
using BolluFlow.Application.Features.Dashboard.GetOrdersByStatus;
using BolluFlow.Application.Features.Dashboard.GetRecentOrders;
using BolluFlow.Application.Features.Dashboard.GetStockAlerts;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BolluFlow.Tests.Integration;

public class DashboardEndpointsTests(DashboardWebApplicationFactory factory) : IClassFixture<DashboardWebApplicationFactory>
{
    [Fact]
    public async Task ShouldReturnConsistentEmptyContracts()
    {
        await ResetDatabaseAsync();
        var client = factory.CreateClient();

        var stockAlertsResponse = await client.GetAsync("/dashboard/stock-alerts");
        var ordersByStatusResponse = await client.GetAsync("/dashboard/orders-by-status");
        var inventoryValueResponse = await client.GetAsync("/dashboard/inventory-value-by-type");
        var recentOrdersResponse = await client.GetAsync("/dashboard/recent-orders");

        Assert.Equal(HttpStatusCode.OK, stockAlertsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, ordersByStatusResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, inventoryValueResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, recentOrdersResponse.StatusCode);

        var stockAlerts = await stockAlertsResponse.Content.ReadFromJsonAsync<List<GetStockAlertsResult>>();
        var ordersByStatus = await ordersByStatusResponse.Content.ReadFromJsonAsync<List<GetOrdersByStatusResult>>();
        var inventoryValue = await inventoryValueResponse.Content.ReadFromJsonAsync<List<GetInventoryValueByTypeResult>>();
        var recentOrders = await recentOrdersResponse.Content.ReadFromJsonAsync<List<GetRecentOrdersResult>>();

        Assert.Empty(stockAlerts!);
        Assert.Equal(5, ordersByStatus!.Count);
        Assert.All(ordersByStatus, x => Assert.Equal(0, x.Total));
        Assert.Equal(
            ["Pending", "InProduction", "Ready", "Delivered", "Canceled"],
            ordersByStatus.Select(x => x.Status).ToArray());

        Assert.Equal(3, inventoryValue!.Count);
        Assert.All(inventoryValue, x => Assert.Equal(0m, x.TotalValue));
        Assert.Empty(recentOrders!);
    }

    [Fact]
    public async Task ShouldReturnDashboardAggregations()
    {
        await ResetDatabaseAsync();
        await SeedDashboardDataAsync();

        var client = factory.CreateClient();

        var stockAlerts = await client.GetFromJsonAsync<List<GetStockAlertsResult>>("/dashboard/stock-alerts?limit=2");
        var ordersByStatus = await client.GetFromJsonAsync<List<GetOrdersByStatusResult>>("/dashboard/orders-by-status");
        var inventoryValues = await client.GetFromJsonAsync<List<GetInventoryValueByTypeResult>>("/dashboard/inventory-value-by-type");
        var recentOrders = await client.GetFromJsonAsync<List<GetRecentOrdersResult>>("/dashboard/recent-orders?limit=1");

        Assert.NotNull(stockAlerts);
        Assert.Equal(2, stockAlerts!.Count);
        Assert.Equal("Critical", stockAlerts[0].Severity);
        Assert.True(stockAlerts[0].StockCoveragePercent <= stockAlerts[1].StockCoveragePercent || stockAlerts[1].Severity == "Warning");

        Assert.NotNull(ordersByStatus);
        Assert.Equal(5, ordersByStatus!.Count);
        Assert.Equal(1, ordersByStatus.Single(x => x.Status == "Pending").Total);
        Assert.Equal(1, ordersByStatus.Single(x => x.Status == "Delivered").Total);
        Assert.Equal(0, ordersByStatus.Single(x => x.Status == "Canceled").Total);

        Assert.NotNull(inventoryValues);
        Assert.Equal(3, inventoryValues!.Count);
        Assert.True(inventoryValues.Single(x => x.Type == "Products").TotalValue > 0);

        Assert.NotNull(recentOrders);
        Assert.Single(recentOrders!);
        Assert.Equal("Pending", recentOrders[0].Status);
        Assert.True(recentOrders[0].TotalItems > 0);
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.OrderItems.RemoveRange(db.OrderItems);
        db.Orders.RemoveRange(db.Orders);
        db.Products.RemoveRange(db.Products);
        db.Ingredients.RemoveRange(db.Ingredients);
        db.Supplies.RemoveRange(db.Supplies);
        db.Customers.RemoveRange(db.Customers);
        db.DeliveryAddresses.RemoveRange(db.DeliveryAddresses);

        await db.SaveChangesAsync();
    }

    private async Task SeedDashboardDataAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var address = DeliveryAddress.Create("Rua A", "10", "Centro", "Cidade", "ST", "12345-000", null);
        var customer = Customer.Create(1, "Maria Silva", "1111", address);
        db.Customers.Add(customer);

        db.Products.Add(Product.Create(1, "Bolo", 20, 8, 10));
        db.Products.Add(Product.Create(1, "Torta", 30, 30, 10));
        db.Ingredients.Add(Ingredient.Create(1, "Farinha", BaseUnit.Gram, 2, 11, 10));
        db.Supplies.Add(Supply.Create(1, "Caixa", BaseUnit.Unit, 1, 4, 10));

        await db.SaveChangesAsync();

        var pendingOrder = Order.Create(customer.Id, 2026);
        pendingOrder.AddItem(1, 2, 20, 10);

        var deliveredOrder = Order.Create(customer.Id, 2025);
        deliveredOrder.AddItem(1, 1, 20, 10);
        deliveredOrder.Status = OrderStatus.Delivered;

        db.Orders.AddRange(pendingOrder, deliveredOrder);

        await db.SaveChangesAsync();
    }
}
