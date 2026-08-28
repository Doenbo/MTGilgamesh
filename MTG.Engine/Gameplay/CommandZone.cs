using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Gameplay;

public class CommandZone
{
    private readonly List<CardInstance> _commanders = [];
    public IReadOnlyList<CardInstance> Commanders => _commanders;

    public void AddCommander(CardInstance commander)
    {
        _commanders.Add(commander);
    }
}
