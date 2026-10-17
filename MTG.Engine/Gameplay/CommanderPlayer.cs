using Microsoft.Extensions.Logging;
using MTG.Core.Decks;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Creator;
using MTG.Engine.Cards;
using MTG.Engine.Zones;
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

    private ICommanderDeck Deck { get; init; } //just to hold the data

    //Zones
    public LibraryZone Library { get; } = new();

    public HandZone Hand { get; } = new();

    public GraveyardZone Graveyard { get; } = new();

    public ExileZone Exile { get; } = new();

    public CommandZone CommandZone { get; init; } = new();


    private CommanderPlayer(string name, int life, ICommanderDeck cd, IPlayerInputProvider pip)
    {
        Name = name;
        LifeTotal = life;
        Deck = cd;
        _inputProvider = pip;
        ManaPool = new ManaPool();
    }

    //TODO ICOMMANDERDECK
    public static Result<CommanderPlayer> Create(string name, int life, ICommanderDeck deck, IPlayerInputProvider pip)
    {
        //TODO pip null??

        //var deck = await DeckCreator.CreateCommanderDeckFromText(path);
        //if (deck.IsFailure)
        //    return deck.ToFailure<CommanderPlayer>();

        var player = new CommanderPlayer(name, life, deck, pip);

        var res = player.InitializeCardInstances(deck);
        if (res.IsFailure) return res.ToFailure<CommanderPlayer>();

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Player {player} created", player);

        return Result<CommanderPlayer>.Success(player);
    }

    public static async Task<Result<CommanderPlayer>> Create(string name, int life, CommanderPrecon cp, IPlayerInputProvider pip)
    {
        //TODO pip null??

        var deck = await DeckCreator.CreateCommanderPrecon(cp);
        if (deck.IsFailure)
            return deck.ToFailure<CommanderPlayer>();

        return Create(name, life, deck.Value, pip);
    }

    private Result InitializeCardInstances(ICommanderDeck deck)
    {
        foreach (var comm in deck.Commander)
        {
            var ci = CardInstance.Create(comm, this);
            if (ci.IsFailure) return ci;
            CommandZone.AddCommander(ci.Value);
        }

        foreach (var card in deck.Cards)
        {
            var ci = CardInstance.Create(card, this);
            if (ci.IsFailure) return ci;
            Library.Add(ci.Value);
        }

        Library.Shuffle();

        for (int i = 0; i < 7; i++)
        {
            _ = DrawCard();
        }

        return Result.Success();
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
            var drawnCard = Library.Draw();

            if (drawnCard == null)
                return Result.Failure("Cannot draw a card: Library is empty!");

            Hand.Add(drawnCard);
        }

        return Result.Success();
    }

    public void AddToHand(CardInstance card)
    {
        Hand.Add(card);
    }

    public void RemoveFromHand(CardInstance card)
    {
        Hand.Remove(card);
    }

    public void AddToGraveyard(CardInstance card)
    {
        Graveyard.Add(card);
    }

    public void RemoveFromGraveyard(CardInstance card)
    {
        Graveyard.Remove(card);
    }

    public void AddToExile(CardInstance card)
    {
        Exile.Add(card);
    }

    public void RemoveFromExile(CardInstance card)
    {
        Exile.Remove(card);
    }

    public Result<List<ManaType>> GetDeckColors()
    {
        throw new NotImplementedException();
    }

    public Task<PlayerAction> GetNextAction(GameContext gc) => _inputProvider.GetNextAction(gc, this);

    public override string ToString() => $"{Name} ({LifeTotal})";
}
