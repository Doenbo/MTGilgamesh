using MTG.Core.Enums;
using MTG.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public class LibraryZone : BaseZone
{
    public override ZoneType Type => ZoneType.Library;
    public override bool IsPublic => false;

    public void Shuffle()
    {
        var rng = Random.Shared;
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
        }
    }

    public CardInstance? Draw()
    {
        if (_cards.Count == 0)
            return null;

        var card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }

    public IReadOnlyList<CardInstance> DrawMany(int count)
    {
        var drawnCards = new List<CardInstance>();
        for (int i = 0; i < count; i++)
        {
            var card = Draw();
            if (card == null) break; // Bibliothek ist leer
            drawnCards.Add(card);
        }
        return drawnCards;
    }

    public CardInstance? PeekTop()
    {
        return _cards.LastOrDefault();
    }

    public IReadOnlyList<CardInstance> PeekTopMany(int count)
    {
        if (count <= 0) return Array.Empty<CardInstance>();
        return _cards.TakeLast(Math.Min(count, _cards.Count)).Reverse().ToList();
    }

    public void AddToTop(CardInstance card)
    {
        ArgumentNullException.ThrowIfNull(card);
        _cards.Add(card);
    }

    public void AddToBottom(CardInstance card)
    {
        ArgumentNullException.ThrowIfNull(card);
        _cards.Insert(0, card);
    }

    public IReadOnlyList<CardInstance> Search(Func<CardInstance, bool> predicate)
    {
        return _cards.Where(predicate).ToList();
    }
}