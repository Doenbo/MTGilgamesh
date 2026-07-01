using MTG.Core.Cards;

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
    public bool IsTapped { get; set; }

    public int DamageMarked { get; set; }
    public int Counters { get; set; }
}
