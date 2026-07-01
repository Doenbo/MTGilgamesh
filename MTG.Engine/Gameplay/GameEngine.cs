using MTG.Engine.TurnSteps;

namespace MTG.Engine.Gameplay;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly IGameDisplay _display;
    private readonly IPlayerInputProvider _inputProvider;

    public GameEngine(GameContext context, IGameDisplay display, IPlayerInputProvider inputProvider)
    {
        _context = context;
        _display = display;
        _inputProvider = inputProvider;
    }

    public async void StartGameLoop()
    {
        _context.ActivePlayer = _context.Players[0];
        _context.PriorityPlayer = _context.Players[0];

        _context.Initialize(_display);

        _context.TransitionTo(new UntapStep());

        while (!IsGameOver())
        {
            PlayerAction action = await _inputProvider.GetNextAction(_context, _context.PriorityPlayer);
            _context.HandleIncomingAction(action);
            CheckStateBasedActions();
        }
    }

    private bool IsGameOver() => _context.Players.Count(p => !p.IsEliminated) <= 1;

    private void CheckStateBasedActions()
    {
        foreach (var player in _context.Players)
        {
            if (player.LifeTotal <= 0 && !player.IsEliminated)
            {
                player.IsEliminated = true;
                _context.RemovePlayerFromGame(player);
                _display.LogElimination(player.Name);
            }
        }
    }
}
