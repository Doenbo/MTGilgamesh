using MTG.Resources.Archidekt;
using MTG.Resources.Enums;

namespace MTG.Resources.Tests;

public class ArchidektDeckImporterTests
{
    [Fact]
    public async Task TestCreateCommanderPreconTT()
    {
        var cp = ArchidektDeckImporter.ImportCommanderPrecon(CommanderPrecon.TokenTriumph);
        Assert.True(cp.IsSuccess);
        Assert.Equal(73, cp.Value.Count); //cause of duplicate lands

        Assert.Contains(cp.Value, card => 
            card.Name == "Emmara, Soul of the Accord" &&
            card.Quantity == 1 &&
            card.Set == "grn" &&
            card.CollectorNumber == "168" &&
            card.Type == "Commander{top}"
        );

        Assert.Contains(cp.Value, card => 
            card.Name == "Ajani, Caller of the Pride" &&
            card.Quantity == 1 &&
            card.Set == "fdn" &&
            card.CollectorNumber == "134" &&
            card.Type == "Counters"
        );

        Assert.Contains(cp.Value, card =>
            card.Name == "Forest" &&
            card.Quantity == 15 &&
            card.Set == "hob" &&
            card.CollectorNumber == "198" &&
            card.Type == "Land"
        );
    }

    [Fact]
    public async Task TestCreateCommanderPreconSAS()
    {
        var cp = ArchidektDeckImporter.ImportCommanderPrecon(CommanderPrecon.ScionsAndSpellcraft);
        Assert.True(cp.IsSuccess);
        Assert.Equal(92, cp.Value.Count); //cause of duplicate lands

        Assert.Contains(cp.Value, card => 
            card.Name == "Y'shtola, Night's Blessed" &&
            card.Quantity == 1 &&
            card.Set == "fic" &&
            card.CollectorNumber == "7" &&
            card.Type == "Commander{top}"
        );

        Assert.Contains(cp.Value, card => 
            card.Name == "Emet-Selch of the Third Seat" &&
            card.Quantity == 1 &&
            card.Set == "fic" &&
            card.CollectorNumber == "81" &&
            card.Type == "Recursion"
        );

        Assert.Contains(cp.Value, card =>
            card.Name == "Island" &&
            card.Quantity == 3 &&
            card.Set == "hob" &&
            card.CollectorNumber == "195" &&
            card.Type == "Land"
        );

        Assert.Contains(cp.Value, card =>
            card.Name == "Hildibrand Manderville // Gentleman's Rise" &&
            card.Quantity == 1 &&
            card.Set == "fic" &&
            card.CollectorNumber == "83" &&
            card.Type == "Tokens"
        );

        Assert.Contains(cp.Value, card =>
            card.Name == "Summon: Good King Mog XII" &&
            card.Quantity == 1 &&
            card.Set == "fic" &&
            card.CollectorNumber == "26" &&
            card.Type == "Tokens"
        );
    }
}
