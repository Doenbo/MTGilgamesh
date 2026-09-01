namespace MTG.Core.Properties;

public class Power
{
    public string Value { get; init; }

    public Power(string power)
    {
        Value = power;
    }

    public static implicit operator int(Power power)
        => int.TryParse(power.Value, out var val) ? val : 0;

    public override string ToString() => Value;
}