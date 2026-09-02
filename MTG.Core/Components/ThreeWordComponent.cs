using MTG.Core.Enums;

namespace MTG.Core.Components;

public record KeywordAbilitiesComponent(IReadOnlyList<KeywordAbility> Abilities) : ICardComponent;
public record KeywordActionsComponent(IReadOnlyList<KeywordAction> Actions) : ICardComponent;
public record AbilityWordsComponent(IReadOnlyList<AbilityWord> Words) : ICardComponent;