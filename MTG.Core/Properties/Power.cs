namespace MTG.Core.Properties;

public class Power
{
    private string Value { get; set; }

    public Power(string power)
    {
        Value = power;
    }

    public override string ToString() => Value;
}
