using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<ResultOf<UserModel>>;