using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Components;

public class ColorComponent : ICardComponent
{
    public ManaType ColorIdentity { get; init; }
    public ManaType ColorIndicator { get; private set; }
    public ManaType Colors { get; private set; }

    private ColorComponent(ManaType colorIdentity, ManaType colorIndicator, ManaType colors)
    {
        ColorIdentity = colorIdentity;
        ColorIndicator = colorIndicator;
        Colors = colors;
    }

    public static Result<ColorComponent> Create(
        List<string>? colorIdentity, List<string>? colorIndicator, List<string>? color)
    {
        //if (colorIdentity == null)
        //    return Result<ColorComponent>.Failure("Must have a Color Identity!");

        var identityResult = ParseOptionalColors(colorIdentity);
        if (identityResult.IsFailure)
            return identityResult.ToFailure<ColorComponent>();

        var indicatorResult = ParseOptionalColors(colorIndicator);
        if (indicatorResult.IsFailure)
            return indicatorResult.ToFailure<ColorComponent>();

        var colorResult = ParseOptionalColors(color);
        if (colorResult.IsFailure)
            return colorResult.ToFailure<ColorComponent>();

        return Result<ColorComponent>.Success(new ColorComponent(identityResult.Value, indicatorResult.Value, colorResult.Value));
    }

    //public static Result<ColorComponent> CreateS(List<string>? colorIndicator, List<string>? color)
    //{
    //    var indicatorResult = ParseOptionalColors(colorIndicator);
    //    if (indicatorResult.IsFailure)
    //        return indicatorResult.ToFailure<ColorComponent>();

    //    var colorResult = ParseOptionalColors(color);
    //    if (colorResult.IsFailure)
    //        return colorResult.ToFailure<ColorComponent>();

    //    return Result<ColorComponent>.Success(new ColorComponent(new Color() /*TODO*/, indicatorResult.Value, colorResult.Value));
    //}

    private static Result<ManaType> ParseStringToEnum(List<string> colorStrings)
    {
        ManaType result = ManaType.Colorless;

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
                default:
                    return Result<ManaType>.Failure($"Color '{str}' is invalid!");
            }
        }

        return Result<ManaType>.Success(result);
    }

    private static Result<ManaType> ParseOptionalColors(List<string>? colorStrings) =>
        colorStrings == null || colorStrings.Count == 0 ? Result<ManaType>.Success(ManaType.Colorless) : ParseStringToEnum(colorStrings);
}
