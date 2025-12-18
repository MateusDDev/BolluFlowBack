using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using Mapster;

namespace BolluFlow.Application.Mapping;

public class UserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserModel>();
    }
}