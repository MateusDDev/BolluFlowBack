using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Services;

public static class SlugService
{
    private static readonly Regex NonAlphaNumericRegex = new("[^a-z0-9]+", RegexOptions.Compiled);
    private static readonly Regex HyphenRegex = new("-{2,}", RegexOptions.Compiled);

    public static string? Generate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        var slug = builder.ToString().Normalize(NormalizationForm.FormC);
        slug = NonAlphaNumericRegex.Replace(slug, "-");
        slug = HyphenRegex.Replace(slug, "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? null : slug;
    }

    public static async Task<string> GenerateUniqueAsync(
        ApplicationDbContext db,
        string baseSlug,
        long? excludedUserId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 2;

        while (await db.Users.AnyAsync(
                   x => x.Slug == slug && (!excludedUserId.HasValue || x.Id != excludedUserId.Value),
                   cancellationToken))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }
}
