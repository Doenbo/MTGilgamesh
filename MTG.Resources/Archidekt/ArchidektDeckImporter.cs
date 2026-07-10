using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Resources.Enums;
using System.Text.RegularExpressions;

namespace MTG.Resources.Archidekt;

public static class ArchidektDeckImporter
{
    private static string pattern =
        @"^(?<quantity>\d+)x\s+(?<name>.+?)\s+\((?<set>[^)]+)\)\s+(?<number>\d+)\s+\[(?<type>[^\]]+)\]";

    public static Result<List<CardRef>> ImportCommanderPrecon(CommanderPrecon cp)
    {
        //TODO Fix path
        string precon_path = Path.GetFullPath($@"C:\Git\MTGilgamesh\MTG.Resources\Archidekt\PreconsText\{cp}.txt");

        return ImportDeckFromText(precon_path);
    }

    public static Result<List<CardRef>> ImportDeckFromText(string path)
    {
        IEnumerable<string> lines;

        try { lines = File.ReadLines(path); }
        catch (Exception ex) { return Result<List<CardRef>>.Failure($"Could not read File: {ex}"); }

        var cardrefs = new List<CardRef>();
        foreach (var line in lines)
        {
            Match match = Regex.Match(line, pattern);

            if (!match.Success) continue; //TODO?

            string quantity = match.Groups["quantity"].Value;
            string name = match.Groups["name"].Value;
            string set = match.Groups["set"].Value;
            string number = match.Groups["number"].Value;
            string type = match.Groups["type"].Value;

            if (!int.TryParse(quantity, out int iQuantity))
                return Result<List<CardRef>>.Failure($"Could not parse int: {quantity}");

            cardrefs.Add(new CardRef() { Quantity = iQuantity, Name = name, Set = set, CollectorNumber = number, Type = type });
        }

        return Result<List<CardRef>>.Success(cardrefs);
    }
}
