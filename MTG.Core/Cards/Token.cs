using MTG.Core.Enums;
using MTG.Core.Wrapper;
using System.Collections.Immutable;

namespace MTG.Core.Cards;

//TODO?
public record TokenDefinition(
    int Power,
    int Toughness,
    ManaType Color,
    ImmutableArray<SubtypeWrapper> Subtypes,
    ImmutableArray<KeywordWrapper> Keywords
);