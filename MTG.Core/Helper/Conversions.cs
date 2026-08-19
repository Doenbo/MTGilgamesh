using System.Text;

namespace MTG.Core.Helper;

public static class Conversions
{

    public static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        input = input.Trim();
        var sb = new StringBuilder(input.Length);
        bool makeUpper = true;

        foreach (var c in input.ToList())
        {
            if (char.IsWhiteSpace(c) || c == '_')
                makeUpper = true;
            else if (c == '\'' || c == '!')
                makeUpper = false;
            else
            {
                sb.Append(makeUpper ? char.ToUpper(c) : c);
                makeUpper = false;
            }
        }

        return sb.ToString();
    }
}
