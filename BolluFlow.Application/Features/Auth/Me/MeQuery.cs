using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Auth.Me;

public record MeQuery : IQuery<ResultOf<UserModel>>;