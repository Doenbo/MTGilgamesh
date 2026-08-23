using MTG.Core.Abilities;
using MTG.Core.Helper;

namespace MTG.Core.Components.OracleText;

public class ProduceManaComponent : ICardComponent
{
    public IReadOnlyList<ManaUnit> ManaUnits { get; init; }
    public bool RequiresTap { get; init; }

    private ProduceManaComponent(IEnumerable<ManaUnit> manaUnits, bool requiresTap)
    {
        ManaUnits = manaUnits.ToList().AsReadOnly();
        RequiresTap = requiresTap;
    }

    public static Result<ProduceManaComponent> Create(IEnumerable<ManaUnit> manaUnits, bool requiresTap)
    {
        var list = manaUnits.ToList();
        if (list.Count == 0)
            return Result<ProduceManaComponent>.Failure("Cannot create ProduceManaComponent without mana units.");

        return Result<ProduceManaComponent>.Success(new ProduceManaComponent(list, requiresTap));
    }
}