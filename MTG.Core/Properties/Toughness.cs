namespace MTG.Core.Properties;

public class Toughness
{
    public string Value { get; init; }

    public Toughness(string toughness)
    {
        Value = toughness;
    }

    public static implicit operator int(Toughness toughness)
        => int.TryParse(toughness.Value, out var val) ? val : 0;

    public override string ToString() => Value;
}
