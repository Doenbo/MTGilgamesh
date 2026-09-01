using Microsoft.Extensions.Logging;
using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Helper;
using MTG.Core.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MTG.Core.OracleTextParsers;

public partial class PlaneswalkerOracleTextParser : ICardTypeOracleParser
{
    private readonly ActivatedAbilityParser _activatedAbilityParser;

    [GeneratedRegex(@"^[\+\−\-\d]+\s*:", RegexOptions.IgnoreCase)]
    private static partial Regex GetLoyaltyAbilityRegex();

    public PlaneswalkerOracleTextParser(ActivatedAbilityParser activatedAbilityParser)
    {
        _activatedAbilityParser = activatedAbilityParser;
    }

    public PlaneswalkerOracleTextParser()
        : this(new ActivatedAbilityParser(new AbilityCostParser(), new EffectParser()))
    { }

    public bool CanHandle(CardContext context)
        => context.HasCardType(CardType.Planeswalker);

    public Result<IReadOnlyList<ICardComponent>> Parse(string oracleText, CardContext context)
    {
        if (string.IsNullOrWhiteSpace(oracleText))
            return Result<IReadOnlyList<ICardComponent>>.Success([]);

        var components = new List<ICardComponent>();
        var lines = oracleText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (GetLoyaltyAbilityRegex().IsMatch(line))
            {
                var abilityResult = _activatedAbilityParser.Parse(line, context);
                if (abilityResult.IsSuccess && abilityResult.Value != null)
                {
                    components.Add(abilityResult.Value);
                }
            }
            else
            {

            }
        }

        if (!string.IsNullOrEmpty(context.StartingLoyalty))
        {
            var loyaltyResult = PlaneswalkerComponent.Create(context.StartingLoyalty);
            if (loyaltyResult.IsSuccess)
            {
                components.Add(loyaltyResult.Value);
            }
        }

        return Result<IReadOnlyList<ICardComponent>>.Success(components.AsReadOnly());
    }
}