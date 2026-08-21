using MTG.Core.Types;

namespace MTG.Core.Abilities;

public record CardContext(
    string CardName,
    IReadOnlyList<CardType> CardTypes,
    IReadOnlyList<string> Subtypes
);