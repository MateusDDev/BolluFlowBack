using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Customers.UpdateCustomer;

public record UpdateCustomerCommand(long Id, UpdateCustomerCommandBody Body) : ICommand<ResultOf<CustomerModel>>;

public record UpdateCustomerCommandBody(
    string? Name,
    string? Whatsapp,
    string? Preferences,
    DateTime? Birthday,
    UpdateCustomerCommandDeliveryAddress? DeliveryAddress);
    
public record UpdateCustomerCommandDeliveryAddress(string Street, int Number, string Neighborhood);