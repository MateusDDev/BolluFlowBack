namespace BolluFlow.Domain.Entities;

public class FixedCost
{
    private FixedCost() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public string StartMonth { get; private set; } = null!;
    public string? EndMonth { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static FixedCost Create(string name, decimal amount, string startMonth, string? endMonth, string? notes)
    {
        var utcNow = DateTime.UtcNow;
        return new FixedCost
        {
            Id = Guid.NewGuid(),
            Name = name,
            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero),
            StartMonth = startMonth,
            EndMonth = endMonth,
            Notes = notes,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
    }

    public void Update(string name, decimal amount, string startMonth, string? endMonth, string? notes)
    {
        Name = name;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        StartMonth = startMonth;
        EndMonth = endMonth;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DeletePermanently()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEndMonth(string? endMonth)
    {
        EndMonth = endMonth;
        UpdatedAt = DateTime.UtcNow;
    }
}
