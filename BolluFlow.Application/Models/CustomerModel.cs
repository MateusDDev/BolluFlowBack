using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Models;

public record CustomerModel(
    long Id,
    long UserId,
    string Name,
    string Whatsapp,
    string? Preferences,
    DateTime? Birthday,
    DeliveryAddressModel? DeliveryAddress
)
{
    public static CustomerModel From(Customer customer)
    {
        return new CustomerModel(
            customer.Id,
            customer.UserId,
            customer.Name,
            customer.Whatsapp,
            customer.Preferences,
            customer.Birthday,
            customer.DeliveryAddress is null ? null : DeliveryAddressModel.From(customer.DeliveryAddress));
    }
}
