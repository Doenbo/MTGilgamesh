using MTG.Core.Enums;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Abilities;

public class ManaUnit
{
    public ManaType ManaFixed { get; private set; }
    public List<ManaType> ManaChoice { get; init; } = [];
    public ManaDynamicType ManaDynamic { get; private set; }

    public bool IsFixed { get; private set; } = false;
    public bool IsChoice { get; private set; } = false;
    public bool IsDynamic { get; private set; } = false;

    public ManaRestriction ManaRestriction { get; init; } = ManaRestriction.None;

    private ManaUnit(ManaRestriction mr = ManaRestriction.None)
    {
        ManaRestriction = mr;
    }

    public static Result<ManaUnit> CreateFixed(ManaType mt, ManaRestriction mr = ManaRestriction.None)
    {
        return Result<ManaUnit>.Success(new ManaUnit(mr)
        {
            ManaFixed = mt,
            IsFixed = true
        });
    }

    public static Result<ManaUnit> CreateChoice(IReadOnlyList<ManaType> lmt, ManaRestriction mr = ManaRestriction.None)
    {
        var mu = new ManaUnit(mr);
        mu.ManaChoice.AddRange(lmt);
        mu.IsChoice = true;
        return Result<ManaUnit>.Success(mu);
    }

    public static Result<ManaUnit> CreateDynamic(ManaDynamicType mdt, ManaRestriction mr = ManaRestriction.None)
    {
        return Result<ManaUnit>.Success(new ManaUnit(mr)
        {
            ManaDynamic = mdt,
            IsDynamic = true
        });
    }

    public Result<ManaUnit> Clone()
    {
        if (IsFixed)
        {
            var mfixed = CreateFixed(ManaFixed, ManaRestriction);
            return mfixed.IsSuccess ? Result<ManaUnit>.Success(mfixed.Value) : mfixed.ToFailure<ManaUnit>();
        }

        if (IsChoice)
        {
            var choice = CreateChoice(ManaChoice, ManaRestriction);
            return choice.IsSuccess ? Result<ManaUnit>.Success(choice.Value) : choice.ToFailure<ManaUnit>();
        }

        if (IsDynamic)
        {
            var dynamic = CreateDynamic(ManaDynamic, ManaRestriction);
            return dynamic.IsSuccess ? Result<ManaUnit>.Success(dynamic.Value) : dynamic.ToFailure<ManaUnit>();
        }

        return Result<ManaUnit>.Failure("ManaUnit has no type!");
    }

    public bool CanPayFor(ManaType requiredColor)
    {
        if (requiredColor == ManaType.Colorless)
            return true;

        if (IsFixed)
            return ManaFixed == requiredColor;

        if (IsChoice)
            return ManaChoice.Contains(requiredColor);

        if (IsDynamic)
            return CanPayForDynamic(requiredColor);

        return false;
    }

    public bool CanPayForDynamic(ManaType requiredColor)
    {
        throw new NotImplementedException();
    }
}
