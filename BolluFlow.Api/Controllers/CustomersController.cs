using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Customers.Create;
using BolluFlow.Application.Features.Customers.List;
using BolluFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ResultOf<List<Customer>>> ListCustomers([FromQuery] ListCustomersQuery query, CancellationToken ct)
    { 
        return await mediator.Send(query, ct);
    }

    [HttpPost]
    public async Task<ResultOf<Customer>> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct);
    }
}