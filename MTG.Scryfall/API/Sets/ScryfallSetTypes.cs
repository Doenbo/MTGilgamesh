using System.Collections.Immutable;

namespace MTG.Scryfall.API.Sets;

public class ScryfallSetTypes
{

    //Or IReadOnlyCollection
    private static readonly ImmutableList<string> allowedTypes = [
        "core",
        "expansion",
        "masters",
        "eternal",
        "alchemy",
        "masterpiece",
        "arsenal",
        "from_the_vault",
        "spellbook",
        "premium_deck",
        "duel_deck",
        "draft_innovation",
        "treasure_chest",
        "commander",
        "planechase",
        "archenemy",
        "vanguard",
        "funny",
        "starter",
        "box",
        "promo",
        "token",
        "memorabilia",
        "minigame"
    ];


}
