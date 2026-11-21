using BolluFlow.Domain.Enums;
using BolluFlow.Domain.ValueObjects;

namespace BolluFlow.Domain.Entities;

public class User
{
    private User() { }

    public long Id { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Slug { get; private set; }
    public string? Whatsapp { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public string? WhatsappMessageTemplate { get; private set; }
    public decimal? IndirectCostRate { get; private set; }
    public int? DeliveryTimeMinutes { get; private set; }
    public string? PixKey { get; private set; }
    public StoreAddress? StoreAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public AppModule EnabledModules { get; private set; } = AppModule.None;
    public bool AllowNegativeStock { get; private set; }
    public DateTime? OnboardingCompletedAt { get; private set; }

    public bool HasModule(AppModule module) => EnabledModules.HasFlag(module);

    public static User Create(
        string name,
        string email,
        string hashedPassword,
        string? profileImageUrl,
        string? slug = null)
    {
        return new User
        {
            IsActive = true,
            Name = name,
            Email = email,
            Password = hashedPassword,
            Slug = slug,
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

    public void SetSlug(string? slug)
    {
        Slug = slug;
    }

    public void SetWhatsapp(string? whatsapp)
    {
        Whatsapp = string.IsNullOrWhiteSpace(whatsapp) ? null : whatsapp.Trim();
    }

    public void SetDeliveryTimeMinutes(int? minutes)
    {
        DeliveryTimeMinutes = minutes;
    }

    public void SetStoreAddress(StoreAddress? storeAddress)
    {
        StoreAddress = storeAddress;
    }

    public void SetPixKey(string? pixKey)
    {
        PixKey = string.IsNullOrWhiteSpace(pixKey) ? null : pixKey.Trim();
    }

    public void SetEnabledModules(AppModule modules)
    {
        EnabledModules = modules & AppModule.All;
    }

    public void SetAllowNegativeStock(bool allowNegativeStock)
    {
        AllowNegativeStock = allowNegativeStock;
    }

    public void CompleteOnboarding(AppModule modules)
    {
        SetEnabledModules(modules);
        OnboardingCompletedAt ??= DateTime.UtcNow;
    }
}
