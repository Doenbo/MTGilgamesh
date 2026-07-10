using Microsoft.Extensions.Logging;
using MTG.Core.Cards;
using MTG.Core.Decks;
using MTG.Core.Helper;
using MTG.Engine.Factories;
using MTG.Resources.Enums;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace MTG.Engine.Gameplay;

public class CommanderPlayer
{
    private static readonly ILogger<CommanderPlayer> _logger = LogManager.GetLogger<CommanderPlayer>();

    public string Name { get; init; }
    public int LifeTotal { get; set; }
    public bool IsEliminated { get; set; }

    private CommanderDeck Deck { get; init; } //just to hold the data

    //Library
    private readonly Stack<CardInstance> _library = [];
    public IEnumerable<CardInstance> Library => _library;

    //Hand
    private readonly List<CardInstance> _hand = [];
    public IReadOnlyList<CardInstance> Hand => _hand;

    //Graveyard
    private readonly List<CardInstance> _graveyard = [];
    public IReadOnlyList<CardInstance> Graveyard => _graveyard;

    //Exile
    private readonly List<CardInstance> _exile = [];
    public IReadOnlyList<CardInstance> Exile => _exile;

    private CommanderPlayer(string name, int life, CommanderDeck cd)
    {
        Name = name;
        LifeTotal = life;
        Deck = cd;
    }

    public static async Task<Result<CommanderPlayer>> Create(string name, int life, CommanderPrecon cp)
    {
        var deck = await DeckCreator.CreateCommanderPrecon(cp);
        if (deck.IsFailure)
            return deck.ToFailure<CommanderPlayer>();

        deck.Value.Shuffle();

        var player = new CommanderPlayer(name, life, deck.Value);

        player.InitializePlayer(deck.Value);

        _logger.LogInformation($"Player {player} created");
        return Result<CommanderPlayer>.Success(player);
    }

    private void InitializePlayer(Deck deck)
    {
        for (int i = 0; i < 7; i++)
        {
            _hand.Add(new CardInstance(deck.Cards[i], this));
        }

        for (int i = deck.Cards.Count - 1; i >= 7; i--)
        {
            _library.Push(new CardInstance(deck.Cards[i], this));
        }
    }

    //Puts a Card from the Library into the Hand
    public Result DrawCard()
    {
        if (!Library.Any())
            return Result.Failure("Library is empty!");

        _hand.Add(_library.Pop());
        return Result.Success();
    }

    public void AddToHand(CardInstance card)
    {
        _hand.Add(card);
    }

    public void RemoveFromHand(CardInstance card)
    {
        _hand.Remove(card);
    }

    public void AddToGraveyard(CardInstance card)
    {
        _graveyard.Add(card);
    }

    public void RemoveFromGraveyard(CardInstance card)
    {
        _graveyard.Remove(card);
    }

    public void AddToExile(CardInstance card)
    {
        _exile.Add(card);
    }

    public void RemoveFromExile(CardInstance card)
    {
        _exile.Remove(card);
    }

    public void PushToLibrary(CardInstance card)
    {
        _library.Push(card);
    }

    public CardInstance PopFromLibrary()
    {
        return _library.Pop();
    }

    public CardInstance PeekLibrary()
    {
        return _library.Peek();
    }

    public override string ToString() => $"{Name} ({LifeTotal})";
}
