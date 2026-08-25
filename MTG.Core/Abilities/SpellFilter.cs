using MTG.Core.Types;

namespace MTG.Core.Abilities;

public record SpellFilter(
    IReadOnlyList<CardType>? RequiredTypes = null,
    IReadOnlyList<CardType>? ExcludedTypes = null,
    IReadOnlyList<string>? RequiredSubtypes = null,
    IReadOnlyList<string>? ExcludedSubtypes = null,
    bool OnlyHistoric = false,       // Artifacts, Legendaries, Sagas
    bool OnlyMulticolor = false
)
{
    public static SpellFilter Any => new();
    public static SpellFilter Noncreature => new(ExcludedTypes: [CardType.Creature]);
}
