using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Menu.CreateMenuOrder;

public class CreateMenuOrderHandler(ApplicationDbContext db) : IRequestHandler<CreateMenuOrderCommand, ResultOf<MenuOrderCreatedModel>>
{
    public async Task<ResultOf<MenuOrderCreatedModel>> Handle(CreateMenuOrderCommand request, CancellationToken cancellationToken)
    {
        var validationError = Validate(request);

        if (validationError is not null)
            return ResultOf<MenuOrderCreatedModel>.BadRequest(validationError);

        var owner = await db.Users
            .FirstOrDefaultAsync(x => x.IsActive && x.Slug == request.Slug, cancellationToken);

        if (owner is null)
            return ResultOf<MenuOrderCreatedModel>.NotFound(new DomainError("MENU_NOT_FOUND", "Cardápio não encontrado"));

        var items = request.Items!;

        var requestedProductIds = items
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();

        var products = await db.Products
            .Where(x =>
                x.UserId == owner.Id &&
                requestedProductIds.Contains(x.Id) &&
                x.IsActive &&
                x.FinalPrice > 0)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        if (products.Count != requestedProductIds.Count)
            return ResultOf<MenuOrderCreatedModel>.BadRequest(new DomainError("MENU_INVALID_PRODUCTS", "Um ou mais produtos informados são inválidos"));

        var customerWhatsapp = request.CustomerWhatsapp!.Trim();
        var customer = await db.Customers
            .Include(x => x.DeliveryAddress)
            .FirstOrDefaultAsync(
                x => x.UserId == owner.Id && x.Whatsapp == customerWhatsapp,
                cancellationToken);

        if (customer is null)
        {
            customer = Customer.Create(
                owner.Id,
                request.CustomerName!.Trim(),
                customerWhatsapp,
                DeliveryAddress.Create(
                    request.CustomerStreet!.Trim(),
                    request.CustomerNumber!.Value,
                    request.CustomerNeighborhood!.Trim()));

            db.Customers.Add(customer);
            await db.SaveChangesAsync(cancellationToken);
        }
        else
        {
            customer.Update(
                request.CustomerName!.Trim(),
                DeliveryAddress.Create(
                    request.CustomerStreet!.Trim(),
                    request.CustomerNumber!.Value,
                    request.CustomerNeighborhood!.Trim()));
        }

        var lastOrderNumber = await db.Orders
            .Where(x => x.Customer.UserId == owner.Id)
            .MaxAsync(x => (int?)x.Number, cancellationToken) ?? 0;

        var order = Order.Create(
            customer.Id,
            lastOrderNumber + 1,
            string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim());

        var componentCosts = await db.ProductComponents
            .AsNoTracking()
            .Include(x => x.Recipe)
            .ThenInclude(x => x!.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .Where(x => requestedProductIds.Contains(x.ProductId))
            .ToListAsync(cancellationToken);

        var componentCostsByProduct = componentCosts
            .GroupBy(x => x.ProductId)
            .ToDictionary(
                x => x.Key,
                x => x.Sum(ProductComponentCostService.CalculateComponentCost));

        foreach (var item in items)
        {
            var product = products[item.ProductId];
            var unitCost = componentCostsByProduct.GetValueOrDefault(item.ProductId, 0m);
            order.AddItem(product.Id, item.Quantity, product.FinalPrice, unitCost);
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<MenuOrderCreatedModel>.Created(new MenuOrderCreatedModel
        {
            OrderId = order.Id,
            OrderNumber = order.Number,
            Message = "Pedido recebido com sucesso!"
        });
    }

    private static DomainError? Validate(CreateMenuOrderCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName) || request.CustomerName.Trim().Length < 2)
            return new DomainError("MENU_ORDER_CUSTOMER_NAME_INVALID", "customerName deve ter no mínimo 2 caracteres");

        if (string.IsNullOrWhiteSpace(request.CustomerWhatsapp))
            return new DomainError("MENU_ORDER_CUSTOMER_WHATSAPP_REQUIRED", "customerWhatsapp é obrigatório");

        if (string.IsNullOrWhiteSpace(request.CustomerStreet))
            return new DomainError("MENU_ORDER_CUSTOMER_STREET_REQUIRED", "customerStreet é obrigatório");

        if (!request.CustomerNumber.HasValue)
            return new DomainError("MENU_ORDER_CUSTOMER_NUMBER_REQUIRED", "customerNumber é obrigatório");

        if (string.IsNullOrWhiteSpace(request.CustomerNeighborhood))
            return new DomainError("MENU_ORDER_CUSTOMER_NEIGHBORHOOD_REQUIRED", "customerNeighborhood é obrigatório");

        if (request.Items is null || request.Items.Count == 0)
            return new DomainError("MENU_ORDER_ITEMS_REQUIRED", "items deve conter ao menos um item");

        if (request.Items.Any(x => x.Quantity <= 0))
            return new DomainError("MENU_ORDER_ITEM_QUANTITY_INVALID", "items.quantity deve ser maior que zero");

        return null;
    }
}
