namespace BolluFlow.Application.Models;

public record CustomerModel(
    long Id,
    long UserId,
    string Name,
    string Whatsapp,
    string? Preferences,
    DateTime? Birthday,
    DeliveryAddressModel DeliveryAddress
);