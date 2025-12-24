using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.GetOrderDetails;

public class GetOrderDetailsHandler(ApplicationDbContext db)
    : IRequestHandler<GetOrderDetailsQuery, ResultOf<GetOrderDetailsResult>>
{
    public async Task<ResultOf<GetOrderDetailsResult>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var orders = await db.Orders
            .Include(x => x.Items)
            .Include(x => x.Customer)
            .ThenInclude(x => x.DeliveryAddress)
            .Select(x => new GetOrderDetailsResult
            {
                Id = x.Id,
                Status = x.Status,
                Items = x.Items.Select(item => new GetOrderDetailsResultItem
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    UnitPrice = item.UnitPrice
                }).ToList(),
                CreatedAt = x.CreatedAt,
                CustomerName = x.Customer.Name,
                Note = x.Note
            })
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

        if (orders is null)
            return ResultOf<GetOrderDetailsResult>.BadRequest(CommonErrors.OrderNotFound);
            
        return ResultOf<GetOrderDetailsResult>.Success(orders);
    }
}