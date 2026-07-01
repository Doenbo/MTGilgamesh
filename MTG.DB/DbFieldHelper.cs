namespace MTG.DB;

public static class DbFieldHelper
{
    //without id
    private static readonly List<string> dbfields = [
        "Lang",
        "CMC",
        "Defense",    //optional
        "Keywords",
        "ManaCost",   //optional
        "Name",
        "OracleText", //optional
        "Power",      //optional
        "Toughness",  //optional
        "TypeLine",
        "SetName"
    ];

    public static string GetPlain() => string.Join(", ", dbfields);

    public static string GetInSqBrackets() => string.Join(", ", dbfields.Select(x => $"[{x}]"));

    public static string GetStartingWithAt() => string.Join(", ", dbfields.Select(x => $"@{x}"));

    public static string GetAssigningList() => string.Join(", ", dbfields.Select(x => $"[{x}] = @{x}"));
}
