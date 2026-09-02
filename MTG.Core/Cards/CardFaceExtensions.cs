using MTG.Core.Components;
using MTG.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Cards;

public static class CardFaceExtensions
{
    public static IReadOnlyList<KeywordAbility> GetKeywordAbilities(this ICardFace face) =>
        face.Components.OfType<KeywordAbilitiesComponent>().FirstOrDefault()?.Abilities ?? [];

    public static IReadOnlyList<KeywordAction> GetKeywordActions(this ICardFace face) =>
        face.Components.OfType<KeywordActionsComponent>().FirstOrDefault()?.Actions ?? [];

    public static IReadOnlyList<AbilityWord> GetAbilityWords(this ICardFace face) =>
        face.Components.OfType<AbilityWordsComponent>().FirstOrDefault()?.Words ?? [];
}