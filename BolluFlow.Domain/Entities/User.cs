namespace BolluFlow.Domain.Entities;

public class User
{
    private User() { }

    public long Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? ProfileImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static User Create(
        string name,
        string email,
        string hashedPassword,
        string? profileImageUrl)
    {
        return new User
        {
            Name = name,
            Email = email,
            Password = hashedPassword,
            ProfileImageUrl = profileImageUrl,
            CreatedAt = DateTime.UtcNow
        };
    }
}