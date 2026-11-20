using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Customers.CreateCustomer;

public record CreateCustomerCommand(
    string Name,
    string Whatsapp,
    string? Preferences,
    DateTime? Birthday,
    CreateCustomerDeliveryAddressModel? DeliveryAddress = null) : ICommand<ResultOf<CustomerModel>>;
    
public record CreateCustomerDeliveryAddressModel(string Street, int Number, string Neighborhood);