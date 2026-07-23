using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTG.Core.Helper;
using MTG.Engine.Enums;
using MTG.Engine.TurnSteps;
using MTG.Resources.Enums;
using System.Numerics;
using System.Text;

namespace MTG.Engine.Gameplay;

public class GameContext
{
    private static readonly ILogger<GameContext> _logger = LogManager.GetLogger<GameContext>();

    //Player
    private readonly List<CommanderPlayer> _players = [];
    public IReadOnlyList<CommanderPlayer> Players => _players;

    //Battlefield
    private readonly List<CardInstance> _battlefield = [];
    public IReadOnlyList<CardInstance> Battlefield => _battlefield;

    //Stack
    private readonly Stack<CardInstance> _stack = [];
    public IEnumerable<CardInstance> Stack => _stack;
    public int StackCount => _stack.Count;


    public TurnStep TurnStep { get; set; } = TurnStep.Untap;
    public CommanderPlayer ActivePlayer { get; set; }
    public CommanderPlayer PriorityPlayer { get; set; }
    public CommanderPlayer PriorityRoundInitiator { get; set; }


    public bool HasPlayedLandThisTurn { get; set; }
    public bool IsEndingTheStep { get; set; }


    public IGameDisplay Display { get; private set; }
    private ITurnStep _currentStep;

    private GameContext() { }

    public async static Task<Result<GameContext>> Create()
    {
        var context = new GameContext();
        var p1 = await CommanderPlayer.Create("Dön", 40, CommanderPrecon.TokenTriumph);
        if (p1.IsFailure)
            return p1.ToFailure<GameContext>();
        context.AddPlayer(p1.Value);

        var p2 = await CommanderPlayer.Create("Nüs", 40, CommanderPrecon.ScionsAndSpellcraft);
        if (p2.IsFailure)
            return p2.ToFailure<GameContext>();
        context.AddPlayer(p2.Value);

        var p3 = await CommanderPlayer.Create("Zag", 40, CommanderPrecon.BlightCurse);
        if (p3.IsFailure)
            return p3.ToFailure<GameContext>();
        context.AddPlayer(p3.Value);

        var p4 = await CommanderPlayer.Create("Mel", 40, CommanderPrecon.DoomPrevails);
        if (p4.IsFailure)
            return p4.ToFailure<GameContext>();
        context.AddPlayer(p4.Value);

        return Result<GameContext>.Success(context);
    }

    public void Initialize(IGameDisplay display)
    {
        if (Display != null) return;
        Display = display ?? throw new ArgumentNullException(nameof(display));
    }

    public void HandleIncomingAction(PlayerAction action)
    {
        _currentStep.HandleAction(this, action);
    }

    public IEnumerable<CardInstance> GetBoardOf(CommanderPlayer player)
    {
        return Battlefield.Where(card => card.Controller == player);
    }
    public void AddPlayer(CommanderPlayer player)
    {
        _players.Add(player);
    }

    public void MoveToBattlefield(CardInstance card)
    {
        _battlefield.Add(card);
    }

    public void PushToStack(CardInstance card)
    {
        _stack.Push(card);
    }

    public CardInstance PopFromStack()
    {
        return _stack.Pop();
    }

    public CardInstance PeekStack()
    {
        return _stack.Peek();
    }

    public void PassPriority()
    {
        int currentIndex = _players.IndexOf(PriorityPlayer);
        int nextIndex = (currentIndex + 1) % _players.Count;

        PriorityPlayer = _players[nextIndex];

        Display?.LogMessage($"Priorität wechselt zu: {PriorityPlayer.Name}");
    }

    public void TransitionTo(ITurnStep nextStep)
    {
        _currentStep?.OnStepExit(this);
        _currentStep = nextStep;

        TurnStep = nextStep.Name;

        _currentStep.OnStepEnter(this);
    }

    public void CastSpellAndStartPriorityRound(PlayerAction action)
    {
        var card = action.TargetCardInstance;
        var player = action.Player;

        player.RemoveFromHand(card);
        PushToStack(card);
        Display.LogMessage($"{player.Name} casts {card.CardData.FullName} (Object is on the STACK!)");

        IsEndingTheStep = true;
        PriorityRoundInitiator = player;

        PassPriority();
    }

    public void ResolveTopStackObject()
    {
        var resolvedCard = PopFromStack();
        Display.LogMessage($"\n=== RESOLVING: {resolvedCard.CardData.FullName} ===");

        if (resolvedCard.CardData.IsPermanent())
        {
            MoveToBattlefield(resolvedCard);
            Display.LogMessage($"{resolvedCard.CardData.FullName} enters the battlefield.");
        }
        else
        {
            resolvedCard.Owner.AddToGraveyard(resolvedCard);
            Display.LogMessage($"{resolvedCard.CardData.FullName} finishes resolving and goes to Graveyard.");
        }

        PriorityPlayer = ActivePlayer;
        PriorityRoundInitiator = ActivePlayer;
        IsEndingTheStep = true;
    }

    public void HandleReactionCast(PlayerAction action)
    {
        var card = action.TargetCardInstance;

        if (card.CardData.IsLand() || !card.CardData.IsInstant())
        {
            Display.LogMessage($"You cannot play {card.CardData.FullName} as a reaction! (Stuck or phase transition in progress)");
            return;
        }

        CastSpellAndStartPriorityRound(action);
    }

    public void AdvanceToNextPlayersTurn()
    {
        int currentIndex = _players.IndexOf(ActivePlayer);
        int nextIndex = (currentIndex + 1) % _players.Count;

        while (_players[nextIndex].IsEliminated)
        {
            nextIndex = (nextIndex + 1) % _players.Count;
        }

        ActivePlayer = _players[nextIndex];
        Display.LogMessage($"It's the turn of {ActivePlayer.Name}");

        HasPlayedLandThisTurn = false;

        TransitionTo(new UntapStep());
    }

    public void RemovePlayerFromGame(CommanderPlayer p)
    {
        _battlefield.RemoveAll(card => card.Owner == p);
    }

    public string ToConsoleBattlefield()
    {
        var sorted = _battlefield.OrderBy(c => c.Owner);
        var sb = new StringBuilder();
        sb.AppendLine($"/------------------------------------\\");
        foreach (var c in sorted)
        {
            sb.AppendLine($"{c.CardData.FullName} [{c.Owner.Name}]");
        }
        return sb.ToString();
    }

    public string ToConsoleStack()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"/------------------------------------\\");
        foreach (var c in _stack)
        {
            sb.AppendLine($"{c.CardData.FullName} [{c.Owner.Name}]");
        }
        return sb.ToString();
    }
}
