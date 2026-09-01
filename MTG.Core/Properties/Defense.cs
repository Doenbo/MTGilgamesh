namespace MTG.Core.Properties;

public class Defense
{
    public string Value { get; init; }

    public Defense(string defense)
    {
        Value = defense;
    }

    public static implicit operator int(Defense defense)
        => int.TryParse(defense.Value, out var val) ? val : 0;

    public override string ToString() => Value;
}
