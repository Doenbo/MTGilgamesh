using Microsoft.Extensions.Logging;
using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Helper;
using MTG.Core.Parser;

namespace MTG.Core.OracleTextParsers;

public interface IOracleTextParser
{
    Result<IReadOnlyList<ICardComponent>> Parse(string oracleText, CardContext cref);
}

public class OracleTextParser(
    IEnumerable<ICardTypeOracleParser> specializedParsers,
    IEnumerable<ILineComponentParser> lineParsers,
    ThreeWordParser threeWordParser) : IOracleTextParser
{
    private static readonly ILogger<OracleTextParser> _logger = LogManager.GetLogger<OracleTextParser>();

    public OracleTextParser(IEnumerable<ILineComponentParser> lineParsers)
        : this([], lineParsers, new ThreeWordParser()) { }

    public OracleTextParser()
        : this([], CreateDefaultParsers(), new ThreeWordParser()) { }

    private static ILineComponentParser[] CreateDefaultParsers()
    {
        var effectParser = new EffectParser();

        return [
            new ProduceManaParser(),
            new ActivatedAbilityParser(new AbilityCostParser(), effectParser),
            new TriggeredAbilityParser(new TriggerConditionParser(), effectParser)
        ];
    }

    public Result<IReadOnlyList<ICardComponent>> Parse(string oracleText, CardContext cref)
    {
        if (string.IsNullOrWhiteSpace(oracleText))
            return Result<IReadOnlyList<ICardComponent>>.Success([]);

        var specializedParser = specializedParsers.FirstOrDefault(p => p.CanHandle(cref));
        if (specializedParser != null)
        {
            return specializedParser.Parse(oracleText, cref);
        }

        var components = new List<ICardComponent>();

        var parsedKeywords = threeWordParser.Parse(oracleText);

        if (parsedKeywords.KeywordAbilities.Count > 0)
            components.Add(new KeywordAbilitiesComponent(parsedKeywords.KeywordAbilities));

        if (parsedKeywords.KeywordActions.Count > 0)
            components.Add(new KeywordActionsComponent(parsedKeywords.KeywordActions));

        if (parsedKeywords.AbilityWords.Count > 0)
            components.Add(new AbilityWordsComponent(parsedKeywords.AbilityWords));

        var lines = oracleText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parser = lineParsers.FirstOrDefault(p => p.CanParse(line));
            if (parser != null)
            {
                var result = parser.Parse(line, cref);

                if (result.IsSuccess && result.Value is not null)
                {
                    components.Add(result.Value);
                }
                else if (result.IsFailure)
                {
                    _logger.LogWarning(
                        "Card context '{CardName}' - Line ignored: \"{Line}\" | Reason: {Error}",
                        cref.Name, line, result.Error);
                }
            }
        }

        return Result<IReadOnlyList<ICardComponent>>.Success(components.AsReadOnly());
    }
}