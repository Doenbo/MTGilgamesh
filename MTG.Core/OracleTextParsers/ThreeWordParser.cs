using MTG.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.OracleTextParsers;

public class ThreeWordParser
{
    public ParsedThreewordsResult Parse(string oracleText)
    {
        if (string.IsNullOrWhiteSpace(oracleText))
        {
            return new ParsedThreewordsResult([], [], []);
        }

        var abilities = new HashSet<KeywordAbility>();
        var actions = new HashSet<KeywordAction>();
        var words = new HashSet<AbilityWord>();

        var lines = oracleText.Split(['\n', '\r'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            ParseAbilityWord(line, words);

            var segments = line.Split([',', '.', ';'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                ParseKeywordAbility(segment, abilities);
                ParseKeywordAction(segment, actions);
            }
        }

        return new ParsedThreewordsResult(
            abilities.ToList().AsReadOnly(),
            actions.ToList().AsReadOnly(),
            words.ToList().AsReadOnly()
        );
    }

    private static void ParseAbilityWord(string line, HashSet<AbilityWord> words)
    {
        var parts = line.Split(['—', '-'], StringSplitOptions.TrimEntries);
        if (parts.Length > 1)
        {
            var candidate = parts[0].Replace(" ", "");
            if (Enum.TryParse<AbilityWord>(candidate, ignoreCase: true, out var word))
            {
                words.Add(word);
            }
        }
    }

    private static void ParseKeywordAbility(string segment, HashSet<KeywordAbility> abilities)
    {
        var firstWord = segment.Split(' ')[0].Trim();
        var fullCleaned = segment.Replace(" ", "");

        if (Enum.TryParse<KeywordAbility>(fullCleaned, ignoreCase: true, out var ability) ||
            Enum.TryParse<KeywordAbility>(firstWord, ignoreCase: true, out ability))
        {
            abilities.Add(ability);
        }
    }

    private static void ParseKeywordAction(string segment, HashSet<KeywordAction> actions)
    {
        var wordsInSegment = segment.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in wordsInSegment)
        {
            if (Enum.TryParse<KeywordAction>(word, ignoreCase: true, out var action))
            {
                actions.Add(action);
            }
        }
    }
}