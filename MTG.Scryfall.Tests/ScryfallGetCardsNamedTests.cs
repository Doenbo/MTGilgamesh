using FluentAssertions;
using MTG.Core.Cards;
using MTG.Scryfall._Getter;

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
        json.IsSuccess.Should().BeTrue();
        //TODO MOVE
        //var act = new ScryfallCardConverter().Convert(json.Value);
        //act.IsSuccess.Should().BeTrue();
        //act.Value.Name.Should().Be(name);
    }
}
