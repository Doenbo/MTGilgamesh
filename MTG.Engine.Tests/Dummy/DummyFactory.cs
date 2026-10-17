using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Decks;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;
using MTG.Creator;
using MTG.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MTG.Engine.Tests.Dummy;

public class DummyFactory
{
    public static Result<ICommanderDeck> CreateCommanderDeck()
    {
        var commf = CreateCardFace(PresetCards.SixF());
        if (commf.IsFailure) return commf.ToFailure<ICommanderDeck>();

        var comm = CreateCard(PresetCards.Six(commf.Value));
        if (comm.IsFailure) return comm.ToFailure<ICommanderDeck>();

        var cards = new List<ICard>();
        for (int i = 0; i < 99; i++)
        {
            var cardf = CreateCardFace(PresetCards.NameF($"Name{i}"));
            if (cardf.IsFailure) return cardf.ToFailure<ICommanderDeck>();

            var card = CreateCard(PresetCards.Name(commf.Value, $"Name{i}"));
            if (card.IsFailure) return card.ToFailure<ICommanderDeck>();

            cards.AddRange(card.Value);
        }

        var args = new CommanderDeckCreationArgs
        {
            //TODO auch dummies?
            Cards = cards,
            Tokens = [],
            Commander = comm.Value,
        };

        return CommanderDeckFactory.Create(args);
    }

    private static Result<List<ICardFace>> CreateCardFace(CardFaceCreationArgs args)
    {
        var cf = CardFaceFactory.Create(args);
        if (cf.IsFailure) return cf.ToFailure<List<ICardFace>>();
        return Result<List<ICardFace>>.Success([cf.Value]);
    }

    public static Result<List<ICard>> CreateCard(CardCreationArgs args)
    {
        var c = CardFactory.Create(args);
        if (c.IsFailure) return c.ToFailure<List<ICard>>();
        return Result<List<ICard>>.Success([c.Value]);
    }

    private static Result<ICard> CreateCommander(CardCreationArgs args)
    {
        return CardFactory.Create(args);
    }

    public static Result<CommanderPlayer> CreateCommanderPlayer(string name = "TestPlayer", int life = 40)
    {
        var d1 = CreateCommanderDeck();
        if (d1.IsFailure) return d1.ToFailure<CommanderPlayer>();
        var d2 = CreatePlayerInputProvider();
        if (d2.IsFailure) return d2.ToFailure<CommanderPlayer>();

        return CommanderPlayer.Create(name, life, d1.Value, d2.Value);
    }

    public static Result<IPlayerInputProvider> CreatePlayerInputProvider()
    {
        return Result<IPlayerInputProvider>.Success(new DummyInputProvider());
    }

    private class DummyInputProvider : IPlayerInputProvider
    {
        public Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
