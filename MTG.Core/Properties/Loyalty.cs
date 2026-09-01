namespace MTG.Core.Properties;

public class Loyalty
{
    public string Value { get; init; }

    public Loyalty(string loyalty)
    {
        Value = loyalty;
    }

    public static implicit operator int(Loyalty loyalty)
        => int.TryParse(loyalty.Value, out var val) ? val : 0;

    public override string ToString() => Value;
}
