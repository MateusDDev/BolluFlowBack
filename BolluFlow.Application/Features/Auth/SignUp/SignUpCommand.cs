using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Auth.SignUp;

public record SignUpCommand(
    string Email,
    string Password,
    string Name,
    string? ProfileImageUrl) : ICommand<ResultOf<UserModel>>;