using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.GetOrderDetails;

public record GetOrderDetailsQuery(long OrderId) : IQuery<ResultOf<GetOrderDetailsResult>>;