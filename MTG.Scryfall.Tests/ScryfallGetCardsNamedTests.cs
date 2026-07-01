using MTG.Core.Cards;
using MTG.Scryfall._Getter;
using MTG.Scryfall.Helper;

namespace MTG.Scryfall.Tests;

public class ScryfallGetCardsNamedTests
{
    [Theory]
    [InlineData("Canopy Vista")]
    public async Task TestGetExactTestSpace(string name)
    {
        var cref = new CardRef() { Name = name };
        var sf = new ScryfallGetCardsNamed();
        var json = await sf.GetExact(cref);
        Assert.True(json.IsSuccess);
        var act = ScryfallCardConverter.Convert(json.Value);
        Assert.True(act.IsSuccess);
        Assert.Equal(name, act.Value.Name);
    }
}
