using MTG.Core.Enums;
using MTG.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public interface IGameZone
{
    ZoneType Type { get; }
    IReadOnlyList<CardInstance> Cards { get; }
    int Count { get; }
    bool IsPublic { get; }

    void Add(CardInstance card);
    bool Remove(CardInstance card);
    bool Contains(CardInstance card);
}