using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Parser;
using System.Text.RegularExpressions;

namespace MTG.Core.Components;

public partial class ActivatedAbilityComponent : ICardComponent
{
    [GeneratedRegex(@"^(?<cost>.+?):\s*(?<effect>.+)$", RegexOptions.IgnoreCase)]
    private static partial Regex ActivatedAbilityRegex();

    public IReadOnlyList<IAbilityCost> Costs { get; }
    public IEffect Effect { get; }
    public TargetRequirement? TargetRequirement { get; }
    public TimingRestriction Timing { get; }

    private ActivatedAbilityComponent(IReadOnlyList<IAbilityCost> costs, IEffect effect)
    {
        Costs = costs;
        Effect = effect;
    }

    public static Result<ActivatedAbilityComponent?> Create(ICard card) => Create(card.MainFace.OracleText);

    public static Result<ActivatedAbilityComponent?> Create(string oracleText)
    {
        var match = ActivatedAbilityRegex().Match(oracleText);
        if (!match.Success)
            return Result<ActivatedAbilityComponent?>.Success(null);

        var rawCost = match.Groups["cost"].Value;
        var rawEffect = match.Groups["effect"].Value;

        var costs = new AbilityCostParser().Parse(rawCost);
        if (costs.IsFailure)
            return costs.ToFailure<ActivatedAbilityComponent?>();

        var effect = new EffectParser().Parse(rawEffect);
        if (effect.IsFailure)
            return effect.ToFailure<ActivatedAbilityComponent?>();

        var aac = new ActivatedAbilityComponent(costs.Value, effect.Value);
        return Result<ActivatedAbilityComponent?>.Success(aac);


    }
}