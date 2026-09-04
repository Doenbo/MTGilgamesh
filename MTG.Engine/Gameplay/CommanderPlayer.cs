using Microsoft.Extensions.Logging;
using MTG.Core.Decks;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Engine.Cards;
using MTG.Engine.Decks;
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

        InitializeCommandZone(cd);
    }

    public static async Task<Result<CommanderPlayer>> Create(string name, int life, CommanderPrecon cp, IPlayerInputProvider pip)
    {
        var deck = await DeckCreator.CreateCommanderPrecon(cp);
        if (deck.IsFailure)
            return deck.ToFailure<CommanderPlayer>();

        var player = new CommanderPlayer(name, life, deck.Value, pip);

        player.InitializePlayer(deck.Value);

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Player {player} created", player);

        return Result<CommanderPlayer>.Success(player);
    }

    private void InitializePlayer(ICommanderDeck deck)
    {
        foreach (var card in deck.Cards)
        {
            Library.Add(new CardInstance(card, this));
        }

        Library.Shuffle();

        for (int i = 0; i < 7; i++)
        {
            _ = DrawCard();
        }
    }

    private void InitializeCommandZone(ICommanderDeck cd)
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
