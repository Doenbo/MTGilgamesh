using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;
using System.ComponentModel.Design;

namespace MTG.Core.Decks;

public record CommanderDeckCreationArgs
{
    public required List<ICard> Cards { get; init; }
    public required List<ICard> Tokens { get; init; }
    public required List<ICard> Commander { get; init; }
}

public static class CommanderDeckFactory
{
    public static Result<ICommanderDeck> Create(CommanderDeckCreationArgs args)
    {
        if (NullGuard.HasNullProperty(args, out var nullProperty))
            return Result<ICommanderDeck>.Failure($"CommanderDeck creation failed: '{nullProperty}' cannot be null!");

        if (args.Commander.Count != 1 && args.Commander.Count != 2)
            return Result<ICommanderDeck>.Failure($"Cannot have more than two Commanders! {args.Commander.ToString}");

        if (args.Commander.Count + args.Cards.Count != 100)
            return Result<ICommanderDeck>.Failure($"A Commander Deck needs exactly 100 Cards! " +
                $"Commanders: {args.Commander.Count} / Cards: {args.Cards.Count}");

        return Result<ICommanderDeck>.Success(new CommanderDeck(args.Cards, args.Tokens, args.Commander));
    }

    private class CommanderDeck : Deck, ICommanderDeck
    {
        public CommanderDeck(IReadOnlyList<ICard> cards, IReadOnlyList<ICard> tokens, IReadOnlyList<ICard> commander) : base(cards, tokens)
        {
            Commander = commander;
        }

        public IReadOnlyList<ICard> Commander { get; init; }

        public Result<ICard> GetRandomCard()
        {
            if (Cards.Count == 0)
                return Result<ICard>.Failure("Deck is empty.");

            return Result<ICard>.Success(Cards[Random.Shared.Next(0, Cards.Count)]);
        }

        public Result<ManaType> GetDeckColorIdentity()
        {
            ManaType result = ManaType.None;

            foreach (ICard comm in Commander)
            {
                result |= comm.ColorIdentity;
            }

            return Result<ManaType>.Success(result);
        }

        public Result<bool> IsValidCommanderDeck()
        {
            var deckColor = GetDeckColorIdentity();
            if (deckColor.IsFailure)
                return deckColor.ToFailure<bool>();

            foreach (var card in Commander)
            {
                if (card.Legalities.TryGetValue(Format.Commander, out var legality) && legality != Legality.Legal)
                    return Result<bool>.Failure($"Card '{card}' is not legal in Commander!");
            }

                foreach (var card in Cards)
            {
                if (card.Legalities.TryGetValue(Format.Commander, out var legality) && legality != Legality.Legal)
                    return Result<bool>.Failure($"Card '{card}' is not legal in Commander!");

                var isLegal = (card.ColorIdentity & ~deckColor.Value) == ManaType.None;
                if (!isLegal)
                    return Result<bool>.Failure($"Illegal Card for color identity: {card}");
            }

            return Result<bool>.Success(true);
        }
    }
}