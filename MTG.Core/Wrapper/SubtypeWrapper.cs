using MTG.Core.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MTG.Core.Wrapper;

public readonly record struct SubtypeWrapper
{
    public CreatureType? Creature { get; }
    public ArtifactType? Artifact { get; }
    public EnchantmentType? Enchantment { get; }
    public PlaneswalkerType? Planeswalker { get; }

    public SubtypeWrapper(CreatureType type) => Creature = type;
    public SubtypeWrapper(ArtifactType type) => Artifact = type;
    public SubtypeWrapper(EnchantmentType type) => Enchantment = type;
    public SubtypeWrapper(PlaneswalkerType type) => Planeswalker = type;

    public static implicit operator SubtypeWrapper(CreatureType type) => new(type);
    public static implicit operator SubtypeWrapper(ArtifactType type) => new(type);
    public static implicit operator SubtypeWrapper(EnchantmentType type) => new(type);
    public static implicit operator SubtypeWrapper(PlaneswalkerType type) => new(type);

    public static bool TryParse(string rawValue, out SubtypeWrapper result)
    {
        if (Enum.TryParse<CreatureType>(rawValue, ignoreCase: true, out var creature))
        {
            result = creature;
            return true;
        }
        if (Enum.TryParse<ArtifactType>(rawValue, ignoreCase: true, out var artifact))
        {
            result = artifact;
            return true;
        }
        if (Enum.TryParse<EnchantmentType>(rawValue, ignoreCase: true, out var enchantment))
        {
            result = enchantment;
            return true;
        }
        if (Enum.TryParse<PlaneswalkerType>(rawValue, ignoreCase: true, out var planeswalker))
        {
            result = planeswalker;
            return true;
        }

        result = default;
        return false;
    }
}
