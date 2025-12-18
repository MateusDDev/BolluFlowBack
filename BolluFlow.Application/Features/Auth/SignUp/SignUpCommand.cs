using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Auth.SignUp;

public class SignUpCommand : ICommand<ResultOf<UserModel>>
{
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public string Name { get; set; }
    
    public string? ProfileImageUrl { get; set; }
}