using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Customers._DTO;
using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Features.Customers.Create;

public record CreateCustomerCommand(
    string Name,
    string Whatsapp,
    string? Preferences,
    CreateDeliveryAddressDto DeliveryAddress) : ICommand<ResultOf<Customer>>;