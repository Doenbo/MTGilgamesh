namespace MTG.Core.Helper;

public static class FloatExtensions
{
    private const float DefaultEpsilon = 0.001f;

    public static bool IsNotEqualTo(this float a, float b, float epsilon = DefaultEpsilon)
    {
        return Math.Abs(a - b) > epsilon;
    }
}
