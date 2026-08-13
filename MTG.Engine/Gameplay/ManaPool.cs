using MTG.Core.Abilities;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Gameplay;

public class ManaPool
{
    //Mana
    private readonly List<ManaUnit> _mana = [];
    public IReadOnlyList<ManaUnit> Mana => _mana.AsReadOnly();

    public int TotalMana => _mana.Count;

    public void AddMana(ManaUnit manaUnit)
    {
        _mana.Add(manaUnit);
    }

    public void AddMana(IEnumerable<ManaUnit> manaList)
    {
        _mana.AddRange(manaList);
    }

    public bool TryDeduct(ManaType type, ManaRestriction currentContext = ManaRestriction.None)
    {
        var candidate = _mana
            .Where(m => m.CanPayFor(type) && IsRestrictionSatisfied(m.ManaRestriction, currentContext))
            .OrderBy(m => m.ManaRestriction != ManaRestriction.None)
            .FirstOrDefault();

        if (candidate == null) return false;

        _mana.Remove(candidate);
        return true;
    }

    public bool TryDeductGeneric(int amount, ManaRestriction currentContext = ManaRestriction.None)
    {
        var validMana = _mana
            .Where(m => IsRestrictionSatisfied(m.ManaRestriction, currentContext))
            .OrderBy(m => m.ManaRestriction != ManaRestriction.None)
            .Take(amount)
            .ToList();

        if (validMana.Count < amount) return false;

        foreach (var mana in validMana)
        {
            _mana.Remove(mana);
        }

        return true;
    }

    private static bool IsRestrictionSatisfied(ManaRestriction manaRestriction, ManaRestriction currentContext)
    {
        if (manaRestriction == ManaRestriction.None) return true;
        return manaRestriction == currentContext;
    }

    public void Clear()
    {
        _mana.Clear();
    }

    public Result<ManaPool> Clone()
    {
        var copy = new ManaPool();
        foreach (var mu in _mana)
        {
            var nu = mu.Clone();
            if (nu.IsFailure) return nu.ToFailure<ManaPool>();
            copy.AddMana(nu.Value);
        }
        return Result<ManaPool>.Success(copy);
    }

    public string ToStringConsole() =>
        $"Pool: {string.Join(",", Mana)} TotalMana: {TotalMana}";
}