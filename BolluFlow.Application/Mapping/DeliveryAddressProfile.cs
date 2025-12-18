using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using Mapster;

namespace BolluFlow.Application.Mapping;

public class DeliveryAddressProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DeliveryAddress, DeliveryAddressModel>();
    }
}