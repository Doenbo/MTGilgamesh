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
    // Statische Helper für den schnellen Zugriff
    public static CardFilter Any => new();

    public static CardFilter ForType(CardType type)
        => new(RequiredTypes: [type]);

    public static CardFilter ForSubtype(string subtype)
        => new(RequiredSubtypes: [subtype]);

    // Value-Equality für Sammlungen, damit xUnit-Tests immer grün werden
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