namespace MTG.Core.Properties;

public class Loyalty
{
    private string Value { get; set; }

    public Loyalty(string loyalty)
    {
        Value = loyalty;
    }

    public override string ToString() => Value;
}
