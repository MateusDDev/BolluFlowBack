using BolluFlow.Application.Features.Customers.CreateCustomer;
using BolluFlow.Application.Features.Customers.DeleteCustomer;
using BolluFlow.Application.Features.Customers.ListCustomers;
using BolluFlow.Application.Features.Customers.UpdateCustomer;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CustomerModel>>> ListCustomers(CancellationToken ct)
    { 
        var query = new ListCustomersQuery();
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<CustomerModel>> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPut("{customerId:long}")]
    public async Task<ActionResult<CustomerModel>> UpdateCustomer(
        [FromRoute] long customerId,
        [FromBody] UpdateCustomerCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateCustomerCommand(customerId, body);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteCustomer([FromRoute] long id, CancellationToken ct)
    {
        var command = new DeleteCustomerCommand(id);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
}