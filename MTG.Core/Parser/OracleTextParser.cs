using Microsoft.Extensions.Logging;
using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public class OracleTextParser : IOracleTextParser
{
    private readonly IEnumerable<ILineComponentParser> _lineParsers;
    private static readonly ILogger<OracleTextParser> _logger = LogManager.GetLogger<OracleTextParser>();

    public OracleTextParser(IEnumerable<ILineComponentParser> lineParsers)
    {
        _lineParsers = lineParsers;
    }

    public OracleTextParser() : this(CreateDefaultParsers())
    { }

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

        var components = new List<ICardComponent>();
        var lines = oracleText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parser = _lineParsers.FirstOrDefault(p => p.CanParse(line));
            if (parser != null)
            {
                var result = parser.Parse(line, cref);

                if (result.IsSuccess && result.Value is not null)
                {
                    components.Add(result.Value);
                }
                else if (result.IsFailure)
                {
                    // TODO Currently No Error but Logging -> Change?
                    _logger.LogWarning(
                        "Card context '{CardName}' - Line ignored: \"{Line}\" | Reason: {Error}",
                        cref.Name, line, result.Error);
                }
            }
        }

        return Result<IReadOnlyList<ICardComponent>>.Success(components.AsReadOnly());
    }
}