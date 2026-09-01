using System.Collections.Immutable;
using MTG.Core.Helper;
using MTG.Core.Types;
using MTG.Core.Wrapper;

namespace MTG.Core.Abilities;

public record CardFilter : IEquatable<CardFilter>
{
    public ImmutableArray<CardType> RequiredTypes { get; init; } = ImmutableArray<CardType>.Empty;
    public ImmutableArray<SubtypeWrapper> RequiredSubtypes { get; init; } = ImmutableArray<SubtypeWrapper>.Empty;
    public ImmutableArray<CardType> ExcludedTypes { get; init; } = ImmutableArray<CardType>.Empty;
    public ImmutableArray<SubtypeWrapper> ExcludedSubtypes { get; init; } = ImmutableArray<SubtypeWrapper>.Empty;
    public bool OnlyControlledByYou { get; init; }
    public bool OnlyControlledByOpponent { get; init; }
    public bool ExcludeTokens { get; init; }

    public CardFilter() { }

    public CardFilter(
        ImmutableArray<CardType> RequiredTypes = default,
        ImmutableArray<SubtypeWrapper> RequiredSubtypes = default,
        ImmutableArray<CardType> ExcludedTypes = default,
        ImmutableArray<SubtypeWrapper> ExcludedSubtypes = default,
        bool OnlyControlledByYou = false,
        bool OnlyControlledByOpponent = false,
        bool ExcludeTokens = false)
    {
        this.RequiredTypes = RequiredTypes.IsDefault ? ImmutableArray<CardType>.Empty : RequiredTypes;
        this.RequiredSubtypes = RequiredSubtypes.IsDefault ? ImmutableArray<SubtypeWrapper>.Empty : RequiredSubtypes;
        this.ExcludedTypes = ExcludedTypes.IsDefault ? ImmutableArray<CardType>.Empty : ExcludedTypes;
        this.ExcludedSubtypes = ExcludedSubtypes.IsDefault ? ImmutableArray<SubtypeWrapper>.Empty : ExcludedSubtypes;
        this.OnlyControlledByYou = OnlyControlledByYou;
        this.OnlyControlledByOpponent = OnlyControlledByOpponent;
        this.ExcludeTokens = ExcludeTokens;
    }

    public static CardFilter Any => new();

    public static Result<CardFilter> Parse(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return Result<CardFilter>.Success(Any);

        string normalized = rawText.Trim().ToLowerInvariant();
        var requiredTypes = new List<CardType>();
        var excludedTypes = new List<CardType>();

        if (normalized.Contains("nonland"))
        {
            excludedTypes.Add(CardType.Land);
        }

        foreach (CardType type in Enum.GetValues<CardType>())
        {
            string typeName = type.ToString().ToLowerInvariant();
            if (normalized.Contains(typeName))
            {
                requiredTypes.Add(type);
            }
        }

        var filter = new CardFilter(
            RequiredTypes: [.. requiredTypes],
            ExcludedTypes: [.. excludedTypes]
        );

        return Result<CardFilter>.Success(filter);
    }

    public virtual bool Equals(CardFilter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return RequiredTypes.SequenceEqual(other.RequiredTypes) &&
               RequiredSubtypes.SequenceEqual(other.RequiredSubtypes) &&
               ExcludedTypes.SequenceEqual(other.ExcludedTypes) &&
               ExcludedSubtypes.SequenceEqual(other.ExcludedSubtypes) &&
               OnlyControlledByYou == other.OnlyControlledByYou &&
               OnlyControlledByOpponent == other.OnlyControlledByOpponent &&
               ExcludeTokens == other.ExcludeTokens;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var t in RequiredTypes) hash.Add(t);
        foreach (var s in RequiredSubtypes) hash.Add(s);
        foreach (var t in ExcludedTypes) hash.Add(t);
        foreach (var s in ExcludedSubtypes) hash.Add(s);
        hash.Add(OnlyControlledByYou);
        hash.Add(OnlyControlledByOpponent);
        hash.Add(ExcludeTokens);
        return hash.ToHashCode();
    }
}