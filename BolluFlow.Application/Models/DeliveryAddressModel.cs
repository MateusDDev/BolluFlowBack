namespace BolluFlow.Application.Models;

public record DeliveryAddressModel(
    string Street,
    int Number,
    string Neighborhood);