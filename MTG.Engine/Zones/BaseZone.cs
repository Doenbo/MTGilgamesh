using MTG.Core.Enums;
using MTG.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public abstract class BaseZone
{
    protected readonly List<CardInstance> _cards = [];

    public abstract ZoneType Type { get; }
    public abstract bool IsPublic { get; }

    public int Count => _cards.Count;
    public IReadOnlyList<CardInstance> Cards => _cards.AsReadOnly();

    public virtual void Add(CardInstance card)
    {
        _cards.Add(card);
    }

    public virtual bool Remove(CardInstance card)
    {
        return _cards.Remove(card);
    }

    public bool Contains(CardInstance card) => _cards.Contains(card);
}