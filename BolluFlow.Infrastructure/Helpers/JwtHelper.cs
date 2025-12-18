using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BolluFlow.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BolluFlow.Infrastructure.Helpers;

public class JwtHelper(IConfiguration config)
{
    private readonly string _secret = config.GetSection("Jwt:Key").Value ?? throw new ArgumentNullException();
    private readonly string _issuer = config.GetSection("Jwt:Issuer").Value ?? throw new ArgumentNullException();
    private readonly string _audience = config.GetSection("Jwt:Audience").Value ?? throw new ArgumentNullException();

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
