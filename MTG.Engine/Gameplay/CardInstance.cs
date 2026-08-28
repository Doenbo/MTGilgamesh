using MTG.Core.Cards;
using MTG.Core.Enums;
using System.Numerics;

namespace MTG.Engine.Gameplay;

public class CardInstance
{
    public CardInstance(ICard card, CommanderPlayer owner)
    {
        CardData = card;
        Owner = owner;
        Controller = owner;
    }

    public ICard CardData { get; set; }

    public CommanderPlayer Owner { get; set; }
    public CommanderPlayer Controller { get; set; }

    public bool IsTapped { get; set; } = false;
    public bool HasSummoningSickness { get; set; } = true;

    public int DamageMarked { get; set; }
    public Dictionary<MarkerType, int> Counters { get; } = [];


    public void AddCounter(MarkerType type, int amount = 1)
    {
        Counters.TryGetValue(type, out int current);
        Counters[type] = current + amount;
    }

    public override string ToString()
    {
        return CardData.FullName;
    }
}
