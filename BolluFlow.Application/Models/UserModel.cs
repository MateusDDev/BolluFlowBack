using BolluFlow.Application.Services;
using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Models;

public class UserModel
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }

    public string? Slug { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<string> EnabledModules { get; set; } = [];

    public bool AllowNegativeStock { get; set; }

    public DateTime? OnboardingCompletedAt { get; set; }

    public string Token { get; set; } = "";

    public static UserModel From(User user, string token = "")
    {
        return new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            ProfileImageUrl = user.ProfileImageUrl,
            Slug = user.Slug,
            CreatedAt = user.CreatedAt,
            EnabledModules = AppModuleNames.ToNames(user.EnabledModules),
            AllowNegativeStock = user.AllowNegativeStock,
            OnboardingCompletedAt = user.OnboardingCompletedAt,
            Token = token
        };
    }
}
