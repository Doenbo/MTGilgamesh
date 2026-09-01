using MTG.Core.Types;

namespace MTG.Core.Abilities;

public record CardFilter(
    IReadOnlyList<CardType>? RequiredTypes = null,
    IReadOnlyList<string>? RequiredSubtypes = null,
    IReadOnlyList<CardType>? ExcludedTypes = null,
    IReadOnlyList<string>? ExcludedSubtypes = null,
    bool OnlyControlledByYou = false,
    bool OnlyControlledByOpponent = false,
    bool ExcludeTokens = false
)
{
    public static CardFilter Any => new();

    public static CardFilter ForType(CardType type)
        => new(RequiredTypes: [type]);

    public static CardFilter ForSubtype(string subtype)
        => new(RequiredSubtypes: [subtype]);

    public static CardFilter Parse(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return Any;

        string normalized = rawText.Trim().ToLowerInvariant();
        var requiredTypes = new List<CardType>();
        var excludedTypes = new List<CardType>();

        // Check for "nonland" or other exclusions
        if (normalized.Contains("nonland"))
        {
            excludedTypes.Add(CardType.Land);
        }

        // Match against known CardType enum values
        foreach (CardType type in Enum.GetValues<CardType>())
        {
            string typeName = type.ToString().ToLowerInvariant();
            if (normalized.Contains(typeName))
            {
                requiredTypes.Add(type);
            }
        }

        return new CardFilter(
            RequiredTypes: requiredTypes.Count > 0 ? requiredTypes.AsReadOnly() : null,
            ExcludedTypes: excludedTypes.Count > 0 ? excludedTypes.AsReadOnly() : null
        );
    }

    public virtual bool Equals(CardFilter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return OnlyControlledByYou == other.OnlyControlledByYou &&
               OnlyControlledByOpponent == other.OnlyControlledByOpponent &&
               ExcludeTokens == other.ExcludeTokens &&
               SequenceEqualNullSafe(RequiredTypes, other.RequiredTypes) &&
               SequenceEqualNullSafe(RequiredSubtypes, other.RequiredSubtypes) &&
               SequenceEqualNullSafe(ExcludedTypes, other.ExcludedTypes) &&
               SequenceEqualNullSafe(ExcludedSubtypes, other.ExcludedSubtypes);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OnlyControlledByYou, OnlyControlledByOpponent, ExcludeTokens);
    }

    private static bool SequenceEqualNullSafe<T>(IReadOnlyList<T>? first, IReadOnlyList<T>? second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        return first.SequenceEqual(second);
    }
}