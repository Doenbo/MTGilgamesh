using Microsoft.Extensions.Logging;
using MTG.Core.Decks;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Engine.Factories;
using MTG.Resources.Enums;

namespace MTG.Engine.Gameplay;

public class CommanderPlayer
{
    private readonly IPlayerInputProvider _inputProvider;
    private static readonly ILogger<CommanderPlayer> _logger = LogManager.GetLogger<CommanderPlayer>();

    public string Name { get; init; }
    public int LifeTotal { get; set; }
    public ManaPool ManaPool { get; private set; }
    public bool IsEliminated { get; set; }

    private CommanderDeck Deck { get; init; } //just to hold the data

    //Library
    private readonly Stack<CardInstance> _library = [];
    public IEnumerable<CardInstance> Library => _library;

    //Hand
    private readonly List<CardInstance> _hand = [];
    public IReadOnlyList<CardInstance> Hand => _hand.AsReadOnly();

    //Graveyard
    private readonly List<CardInstance> _graveyard = [];
    public IReadOnlyList<CardInstance> Graveyard => _graveyard.AsReadOnly();

    //Exile
    private readonly List<CardInstance> _exile = [];
    public IReadOnlyList<CardInstance> Exile => _exile.AsReadOnly();

    //CommandZone
    public CommandZone CommandZone { get; init; } = new();


    private CommanderPlayer(string name, int life, CommanderDeck cd, IPlayerInputProvider pip)
    {
        Name = name;
        LifeTotal = life;
        Deck = cd;
        _inputProvider = pip;
        ManaPool = new ManaPool();

        InitializeCommandZone(cd);
    }

    public static async Task<Result<CommanderPlayer>> Create(string name, int life, CommanderPrecon cp, IPlayerInputProvider pip)
    {
        var deck = await DeckCreator.CreateCommanderPrecon(cp);
        if (deck.IsFailure)
            return deck.ToFailure<CommanderPlayer>();

        deck.Value.Shuffle();

        var player = new CommanderPlayer(name, life, deck.Value, pip);

        player.InitializePlayer(deck.Value);

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Player {player} created", player);

        return Result<CommanderPlayer>.Success(player);
    }

    private void InitializePlayer(CommanderDeck deck)
    {
        foreach (var card in deck.Cards)
        {
            _library.Push(new CardInstance(card, this));
        }

        for (int i = 0; i < 7; i++)
        {
            _ = DrawCard();
        }
    }

    private void InitializeCommandZone(CommanderDeck cd)
    {
        var first = cd.GetFirstCommander();
        if (first.IsSuccess)
        {
            CommandZone.AddCommander(new CardInstance(first.Value, this));
        }

        var second = cd.GetSecondCommander();
        if (second.IsSuccess)
        {
            CommandZone.AddCommander(new CardInstance(second.Value, this));
        }
    }

    public void UpdateManaPool(ManaPool mp)
    {
        ManaPool = mp;
    }

    //Puts a Card from the Library into the Hand
    public Result DrawCard(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (!Library.Any())
                return Result.Failure("Library is empty!");

            _hand.Add(_library.Pop());
        }
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

    public Result<List<ManaType>> GetDeckColors()
    {
        throw new NotImplementedException();
    }

    public Task<PlayerAction> GetNextAction(GameContext gc) => _inputProvider.GetNextAction(gc, this);

    public override string ToString() => $"{Name} ({LifeTotal})";
}
