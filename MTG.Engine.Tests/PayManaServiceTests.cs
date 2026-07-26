using MTG.Core.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Tests;

public class PayManaServiceTests
{
    public static IEnumerable<object[]> ValidManaStrings =>
        new List<object[]>
        {
            new object[] { "W", 1 },
            new object[] { "14", 14 },
            new object[] { "W/U", 1 },
            new object[] { "2/B", 2 },
            new object[] { "14/B", 14 },
            new object[] { "G/P", 1 },
            new object[] { "X", 0 }
        };

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestCreateValid(string input, float _)
    {
        //TOODOOOOOOOOOOOOOOOO
        var act = ManaSymbol.Create(input);
        Assert.True(act.IsSuccess);
        Assert.Equal($"{{{input}}}", act.Value.ToString());
    }
}
