using MTG.Core.Enums;

namespace MTG.Run.Tests;

public class CardCreatorTests
{
    [Fact]
    public void TestTryParse()
    {
        var dict = new Dictionary<Format, bool>();
        var list = new List<string>() { "Standard", "Future", "Historic", "Timeless", "Gladiator", "Pioneer", "Modern", "Legacy", "Pauper", "Vintage" };

        for (int i = 0; i < 10; i++)
        {
            _ = Enum.TryParse(list[i], out Format eLegality);
            dict.Add(eLegality, true);
        }
    }
}
