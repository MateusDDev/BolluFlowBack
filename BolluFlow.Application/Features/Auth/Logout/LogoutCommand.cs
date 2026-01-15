using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Auth.Logout;

public record LogoutCommand : ICommand<Result>;