namespace BolluFlow.Application.Features.Customers._DTO;

public record CreateDeliveryAddressDto(
    string Street,
    string Number,
    string Neighborhood);