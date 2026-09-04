using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Decks;

public interface ICommanderDeck
{
    IReadOnlyList<ICard> Cards { get; }

    IReadOnlyList<ICard> Tokens { get; }

    Result<ICard> GetFirstCommander();

    Result<ICard> GetSecondCommander();

    Result<ICard> GetRandomCard();

    Result<ManaType> GetDeckColorIdentity();

    Result<bool> IsValidCommanderDeck();
}
