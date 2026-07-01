namespace MTG.Core.Helper;

public static class ListExtensions
{
    private static readonly Random rng = new();

    // Das 'this' sorgt dafür, dass wir .Shuffle() direkt auf jeder List<T> aufrufen können
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            // Werte elegant per C#-Tupel tauschen
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}