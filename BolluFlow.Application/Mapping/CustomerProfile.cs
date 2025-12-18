using BolluFlow.Application.Features.Customers.Create;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using Mapster;

namespace BolluFlow.Application.Mapping;

public class CustomerProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Customer, CustomerModel>();
        
        config.NewConfig<CreateCustomerCommand, Customer>();
    }
}