using MTG.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Gameplay;

public class ManaPool
{
    public int White { get; private set; }
    public int Blue { get; private set; }
    public int Black { get; private set; }
    public int Red { get; private set; }
    public int Green { get; private set; }
    public int Colorless { get; private set; }

    public int TotalMana => White + Blue + Black + Red + Green + Colorless;

    public int Get(ManaType type) => type switch
    {
        ManaType.White => White,
        ManaType.Blue => Blue,
        ManaType.Black => Black,
        ManaType.Red => Red,
        ManaType.Green => Green,
        ManaType.Colorless => Colorless,
        _ => 0
    };

    public void AddMana(IReadOnlyList<ManaType> manaTypes)
    {
        foreach (ManaType type in manaTypes) { AddMana(type); }
    }

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

    public bool TryDeduct(ManaType type, int amount = 1)
    {
        if (Get(type) < amount) return false;

        switch (type)
        {
            case ManaType.White: White -= amount; break;
            case ManaType.Blue: Blue -= amount; break;
            case ManaType.Black: Black -= amount; break;
            case ManaType.Red: Red -= amount; break;
            case ManaType.Green: Green -= amount; break;
            case ManaType.Colorless: Colorless -= amount; break;
        }
        return true;
    }

    public bool TryDeductGeneric(int amount)
    {
        if (TotalMana < amount) return false;

        int remainingToPay = amount;

        ManaType[] priority = [
            ManaType.Colorless, ManaType.Green, ManaType.Red,
            ManaType.Black, ManaType.Blue, ManaType.White
        ];

        foreach (var type in priority)
        {
            while (Get(type) > 0 && remainingToPay > 0)
            {
                TryDeduct(type, 1);
                remainingToPay--;
            }
            if (remainingToPay == 0) break;
        }

        return remainingToPay == 0;
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

    public ManaPool Clone()
    {
        var copy = new ManaPool();

        if (White > 0) copy.AddMana(ManaType.White, White);
        if (Blue > 0) copy.AddMana(ManaType.Blue, Blue);
        if (Black > 0) copy.AddMana(ManaType.Black, Black);
        if (Red > 0) copy.AddMana(ManaType.Red, Red);
        if (Green > 0) copy.AddMana(ManaType.Green, Green);
        if (Colorless > 0) copy.AddMana(ManaType.Colorless, Colorless);

        return copy;
    }

    public string ToStringConsole()
    {
        return $"White:{White} | Blue:{Blue} | Black:{Black} | Red:{Red} | Green:{Green} | " +
               $"Colorless:{Colorless} | TotalMana:{TotalMana} ";
    }
}