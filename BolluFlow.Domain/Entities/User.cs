namespace BolluFlow.Domain.Entities;

public class User
{
    private User() { }

    public long Id { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? ProfileImageUrl { get; private set; }
    public string? WhatsappMessageTemplate { get; private set; }
    public decimal? IndirectCostRate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static User Create(
        string name,
        string email,
        string hashedPassword,
        string? profileImageUrl)
    {
        return new User
        {
            IsActive = true,
            Name = name,
            Email = email,
            Password = hashedPassword,
            ProfileImageUrl = profileImageUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string? name, string? whatsappMessageTemplate, decimal? indirectCostRate)
    {
        if (name is not null)
            Name = name;

        if (indirectCostRate.HasValue)
            IndirectCostRate = indirectCostRate.Value;
        
        WhatsappMessageTemplate = whatsappMessageTemplate;
    }
}
