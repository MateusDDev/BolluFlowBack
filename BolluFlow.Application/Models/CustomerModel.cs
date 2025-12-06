namespace BolluFlow.Application.Models;

public record CustomerModel(
    long Id,
    string Name,
    string Whatsapp,
    string? Preferences,
    DateTime? Birthday,
    DeliveryAddressModel DeliveryAddress
);