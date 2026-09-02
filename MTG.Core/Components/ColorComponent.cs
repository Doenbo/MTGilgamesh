using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Components;

public class ColorComponent : ICardComponent
{
    public ManaType Colors { get; private set; }
    public ManaType ColorIdentity { get; init; }
    public ManaType ColorIndicator { get; private set; }

    private ColorComponent(ManaType colors, ManaType colorIdentity, ManaType colorIndicator)
    {
        Colors = colors;
        ColorIdentity = colorIdentity;
        ColorIndicator = colorIndicator;
    }

    public static Result<ColorComponent> Create(
        List<string>? color, List<string>? colorIdentity, List<string>? colorIndicator)
    {
        var colorResult = ParseOptionalColors(color);
        if (colorResult.IsFailure)
            return colorResult.ToFailure<ColorComponent>();

        var identityResult = ParseOptionalColors(colorIdentity);
        if (identityResult.IsFailure)
            return identityResult.ToFailure<ColorComponent>();

        var indicatorResult = ParseOptionalColors(colorIndicator);
        if (indicatorResult.IsFailure)
            return indicatorResult.ToFailure<ColorComponent>();

        return Result<ColorComponent>.Success(
            new ColorComponent(colorResult.Value, identityResult.Value, indicatorResult.Value));
    }

    private static Result<ManaType> ParseStringToEnum(List<string> colorStrings)
    {
        ManaType result = ManaType.None;

        foreach (var str in colorStrings)
        {
            var upperStr = str.ToUpperInvariant();

            switch (upperStr)
            {
                case "W": result |= ManaType.White; break;
                case "U": result |= ManaType.Blue; break;
                case "B": result |= ManaType.Black; break;
                case "R": result |= ManaType.Red; break;
                case "G": result |= ManaType.Green; break;
                case "C": result |= ManaType.Colorless; break;
                default:
                    return Result<ManaType>.Failure($"Color '{str}' is invalid!");
            }
        }

        return Result<ManaType>.Success(result);
    }

    private static Result<ManaType> ParseOptionalColors(List<string>? colorStrings) =>
        colorStrings == null || colorStrings.Count == 0
            ? Result<ManaType>.Success(ManaType.None)
            : ParseStringToEnum(colorStrings);
}
