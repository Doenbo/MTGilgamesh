using MTG.Core.Types;

namespace MTG.Core.Abilities;

public record CardContext(
    string Name,
    IReadOnlyList<CardType> CardTypes,
    IReadOnlyList<object> Subtypes,
    string? StartingLoyalty = null)
{
    public CardContext(
        string name,
        IEnumerable<CardType>? cardTypes = null,
        IEnumerable<object>? subtypes = null)
        : this(
            name,
            (cardTypes ?? []).ToList().AsReadOnly(),
            (subtypes ?? []).ToList().AsReadOnly())
    { }

    public static CardContext ForName(string name) => new(name);
}