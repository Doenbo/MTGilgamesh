namespace MTG.Core.Properties;

public class Defense
{
    private string Value { get; set; }

    public Defense(string defense)
    {
        Value = defense;
    }

    public override string ToString() => Value;
}
