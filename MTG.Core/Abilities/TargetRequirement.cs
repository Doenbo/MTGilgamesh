using MTG.Core.Enums;

namespace MTG.Core.Abilities;

public record TargetRequirement(
    int MinTargets,
    int MaxTargets,
    TargetType ValidTypes
);
