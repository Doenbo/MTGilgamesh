using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Components;

public class ColorComponent : ICardComponent
{
    public Color ColorIdentity { get; init; }
    public Color ColorIndicator { get; private set; }
    public Color Colors { get; private set; }

    private ColorComponent(Color colorIdentity, Color colorIndicator, Color colors)
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

    private static Result<Color> ParseStringToEnum(List<string> colorStrings)
    {
        Color result = Color.Colorless;

        foreach (var str in colorStrings)
        {
            var upperStr = str.ToUpperInvariant();

            switch (upperStr)
            {
                case "W": result |= Color.White; break;
                case "U": result |= Color.Blue; break;
                case "B": result |= Color.Black; break;
                case "R": result |= Color.Red; break;
                case "G": result |= Color.Green; break;
                default:
                    return Result<Color>.Failure($"Color '{str}' is invalid!");
            }
        }

        return Result<Color>.Success(result);
    }

    private static Result<Color> ParseOptionalColors(List<string>? colorStrings) =>
        colorStrings == null || colorStrings.Count == 0 ? Result<Color>.Success(Color.Colorless) : ParseStringToEnum(colorStrings);
}
