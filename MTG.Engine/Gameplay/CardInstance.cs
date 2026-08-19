using MTG.Core.Cards;
using System.Diagnostics.PerformanceData;
using MTG.Engine.Enums;

namespace MTG.Engine.Gameplay;

public class CardInstance
{
    public CardInstance(ICard c, CommanderPlayer cp)
    {
        CardData = c;
        Owner = cp;
        Controller = cp;
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
}
