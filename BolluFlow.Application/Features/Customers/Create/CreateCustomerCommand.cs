using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Customers.Create;

public record CreateCustomerCommand(
    string Name,
    long UserId,
    string Whatsapp,
    string? Preferences,
    DateTime? Birthday,
    DeliveryAddressModel DeliveryAddress) : ICommand<ResultOf<CustomerModel>>;