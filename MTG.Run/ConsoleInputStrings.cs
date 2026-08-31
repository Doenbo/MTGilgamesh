using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Run;

public class ConsoleInputStrings
{
    // Standards
    public static readonly string[] stdCommands = ["B", "M", "S"];

    public const string s_board = "B: Show Board";
    public const string s_manap = "M: Show Own Mana Pool";
    public const string s_stack = "S: Show Stack";

    public const string o_passp = "0: Pass Priority";
    public const string o_endph = "0: End Phase";
    public const string o_retur = "0: Return";

    public const string f_passp = $"{s_board} | {s_manap} | {s_stack} | {o_passp}";
    public const string f_endph = $"{s_board} | {s_manap} | {s_stack} | {o_endph}";
    public const string f_retur = $"{s_board} | {s_manap} | {s_stack} | {o_retur}";

    // Cheats
    public static readonly string[] cheatCommands = { "OH", "OL" };
    public const string s_ohand = "OH: Show Opponents Hands";
    public const string s_olibr = "OL: Show Opponents Libraries";
    public const string f_ocheat = $"{s_ohand} | {s_olibr}";

}
