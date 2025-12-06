using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Customers.Update;

public record UpdateCustomerCommand(long Id, UpdateCustomerCommandBody Body) : ICommand<ResultOf<CustomerModel>>;

public record UpdateCustomerCommandBody(
    string? Name,
    string? Whatsapp,
    string? Preferences,
    DateTime? Birthday,
    DeliveryAddressModel? DeliveryAddress);