using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.OracleTextParsers;

namespace MTG.Core.Components;

public class ColorComponent : ICardComponent
{
    public ManaType Colors { get; init; }
    public ManaType ColorIndicator { get; init; }

    private ColorComponent(ManaType colors, ManaType colorIndicator)
    {
        Colors = colors;
        ColorIndicator = colorIndicator;
    }

    public static Result<ColorComponent> Create(
        IManaSymbolParser manaParser,
        List<string>? colors,
        List<string>? colorIndicators)
    {
        var colorsResult = manaParser.ParseColorStrings(colors);
        if (colorsResult.IsFailure)
            return colorsResult.ToFailure<ColorComponent>();

        var indicatorResult = manaParser.ParseColorStrings(colorIndicators);
        if (indicatorResult.IsFailure)
            return indicatorResult.ToFailure<ColorComponent>();

        return Result<ColorComponent>.Success(
            new ColorComponent(colorsResult.Value, indicatorResult.Value));
    }
}
