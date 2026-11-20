using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Models;

public class DeliveryAddressModel
{
    public long Id { get; set; }

    public string Street { get; set; } = null!;

    public int Number { get; set; }

    public string Neighborhood { get; set; } = null!;

    public static DeliveryAddressModel From(DeliveryAddress address)
    {
        return new DeliveryAddressModel
        {
            Id = address.Id,
            Street = address.Street,
            Number = address.Number,
            Neighborhood = address.Neighborhood
        };
    }
}
