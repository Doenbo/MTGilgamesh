using MTG.Core.Enums;
using MTG.Core.Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace MTG.Core.Cards;

//TODO?
public record TokenDefinition(
    int Power,
    int Toughness,
    ManaType Color,
    ImmutableArray<SubtypeWrapper> Subtypes,
    ImmutableArray<KeywordWrapper> Keywords
);