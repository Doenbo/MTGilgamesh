using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Tests;

public class ConversionsTest
{
    [Theory]
    [InlineData("Deathtouch", "Deathtouch")]
    [InlineData("First strike", "FirstStrike")]
    [InlineData("More Than Meets the Eye", "MoreThanMeetsTheEye")]
    [InlineData("art_crop", "ArtCrop")]
    public void TestToCamelCase(string input, string exp)
    {
        var act = Conversions.ToCamelCase(input);
        Assert.Equal(exp, act);
    }

    [Theory]
    [InlineData("Deathtouch", KeywordAbility.Deathtouch)]
    [InlineData("First strike", KeywordAbility.FirstStrike)]
    [InlineData("More Than Meets the Eye", KeywordAbility.MoreThanMeetsTheEye)]
    public void TestKeyword(string s, KeywordAbility exp)
    {
        Assert.True(Enum.TryParse(Conversions.ToCamelCase(s), out KeywordAbility act));
        Assert.Equal(exp, act);
    }

    [Theory]
    [InlineData("png", ImageSize.Png)]
    [InlineData("small", ImageSize.Small)]
    [InlineData("art_crop", ImageSize.ArtCrop)]
    public void TestImageSize(string s, ImageSize exp)
    {
        Assert.True(Enum.TryParse(Conversions.ToCamelCase(s), out ImageSize act));
        Assert.Equal(exp, act);
    }
}
