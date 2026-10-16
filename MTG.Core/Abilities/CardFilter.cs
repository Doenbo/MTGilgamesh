using System.Collections.Immutable;
using MTG.Core.Helper;
using MTG.Core.Types;
using MTG.Core.Wrapper;

namespace MTG.Core.Abilities;

public record CardFilter : IEquatable<CardFilter>
{
    public ImmutableArray<CardType> RequiredTypes { get; init; } = [];
    public ImmutableArray<SubtypeWrapper> RequiredSubtypes { get; init; } = [];
    public ImmutableArray<CardType> ExcludedTypes { get; init; } = [];
    public ImmutableArray<SubtypeWrapper> ExcludedSubtypes { get; init; } = [];
    public bool OnlyControlledByYou { get; init; }
    public bool OnlyControlledByOpponent { get; init; }
    public bool ExcludeTokens { get; init; }
    public bool IsSelf { get; init; }

    public CardFilter(
        ImmutableArray<CardType> RequiredTypes = default,
        ImmutableArray<SubtypeWrapper> RequiredSubtypes = default,
        ImmutableArray<CardType> ExcludedTypes = default,
        ImmutableArray<SubtypeWrapper> ExcludedSubtypes = default,
        bool OnlyControlledByYou = false,
        bool OnlyControlledByOpponent = false,
        bool ExcludeTokens = false,
        bool IsSelf = false)
    {
        this.RequiredTypes = RequiredTypes.IsDefault ? [] : RequiredTypes;
        this.RequiredSubtypes = RequiredSubtypes.IsDefault ? [] : RequiredSubtypes;
        this.ExcludedTypes = ExcludedTypes.IsDefault ? [] : ExcludedTypes;
        this.ExcludedSubtypes = ExcludedSubtypes.IsDefault ? [] : ExcludedSubtypes;
        this.OnlyControlledByYou = OnlyControlledByYou;
        this.OnlyControlledByOpponent = OnlyControlledByOpponent;
        this.ExcludeTokens = ExcludeTokens;
        this.IsSelf = IsSelf;
    }

    public static CardFilter Any => new();
    public static CardFilter Self => new(IsSelf: true);

    public static Result<CardFilter> Parse(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return Result<CardFilter>.Success(Any);

        string normalized = rawText.Trim().ToLowerInvariant();

        if (normalized.StartsWith("this ") || normalized.Contains("it"))
        {
            return Result<CardFilter>.Success(Self);
        }

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
               ExcludeTokens == other.ExcludeTokens &&
               IsSelf == other.IsSelf;
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
        hash.Add(IsSelf);
        return hash.ToHashCode();
    }
}