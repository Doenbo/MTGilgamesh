using MTG.Core.Types;

namespace MTG.Core.Abilities;

public record CardContext(
    string Name,
    IReadOnlyList<CardType>? CardTypes = null,
    IReadOnlyList<string>? Subtypes = null
)
{
    public IReadOnlyList<CardType> Types => CardTypes ?? [];
    public IReadOnlyList<string> SubtypeList => Subtypes ?? [];

    public static CardContext ForName(string name) => new(name);
}