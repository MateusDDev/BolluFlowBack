using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Customers.Create;
using BolluFlow.Application.Features.Customers.Delete;
using BolluFlow.Application.Features.Customers.List;
using BolluFlow.Application.Features.Customers.Update;
using BolluFlow.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ResultOf<List<CustomerModel>>> ListCustomers([FromQuery] ListCustomersQuery query, CancellationToken ct)
    { 
        return await mediator.Send(query, ct);
    }

    [HttpPost]
    public async Task<ResultOf<CustomerModel>> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct);
    }

    [HttpPut("/{id:long}")]
    public async Task<ResultOf<CustomerModel>> UpdateCustomer(
        [FromRoute] long id,
        [FromBody] UpdateCustomerCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateCustomerCommand(id, body);
        
        return await mediator.Send(command, ct);
    }

    [HttpDelete("/{id:long}")]
    public async Task<Result> DeleteCustomer([FromRoute] long id, CancellationToken ct)
    {
        var command = new DeleteCustomerCommand(id);
        
        return await mediator.Send(command, ct);
    }
}