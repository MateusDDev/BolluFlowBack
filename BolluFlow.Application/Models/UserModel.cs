namespace BolluFlow.Application.Models;

public class UserModel
{
    public long Id { get; set; }
    
    public string Email { get; set; }
    
    public string Name { get; set; }
    
    public string? ProfileImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string Token { get; set; }
}