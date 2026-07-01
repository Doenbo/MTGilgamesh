namespace MTG.Core.Properties;

public class Toughness
{
    private string Value { get; set; }

    public Toughness(string toughness)
    {
        Value = toughness;
    }

    public override string ToString() => Value;
}
