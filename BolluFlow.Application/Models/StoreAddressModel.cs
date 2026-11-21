using BolluFlow.Domain.ValueObjects;

namespace BolluFlow.Application.Models;

public class StoreAddressModel
{
    public string Street { get; set; } = null!;
    public int Number { get; set; }
    public string Neighborhood { get; set; } = null!;
    public string? Complement { get; set; }

    public static StoreAddressModel? From(StoreAddress? address)
    {
        return address is null
            ? null
            : new StoreAddressModel
            {
                Street = address.Street,
                Number = address.Number,
                Neighborhood = address.Neighborhood,
                Complement = address.Complement
            };
    }
}
