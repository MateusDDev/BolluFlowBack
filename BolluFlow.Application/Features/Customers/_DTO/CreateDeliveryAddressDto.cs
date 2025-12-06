namespace BolluFlow.Application.Features.Customers._DTO;

public record CreateDeliveryAddressDto(
    string Street,
    int Number,
    string Neighborhood);