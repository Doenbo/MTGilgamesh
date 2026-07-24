using MTG.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Properties;

public class ManaPool
{
    public int White { get; private set; }
    public int Blue { get; private set; }
    public int Black { get; private set; }
    public int Red { get; private set; }
    public int Green { get; private set; }
    public int Colorless { get; private set; }

    public int TotalMana => White + Blue + Black + Red + Green + Colorless;

    public void AddMana(ManaType type, int amount = 1)
    {
        switch (type)
        {
            case ManaType.White: White += amount; break;
            case ManaType.Blue: Blue += amount; break;
            case ManaType.Black: Black += amount; break;
            case ManaType.Red: Red += amount; break;
            case ManaType.Green: Green += amount; break;
            case ManaType.Colorless: Colorless += amount; break;
        }
    }

    public void Clear()
    {
        White = 0;
        Blue = 0;
        Black = 0;
        Red = 0;
        Green = 0;
        Colorless = 0;
    }
}