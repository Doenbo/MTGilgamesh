using MTG.Engine.Events;
using MTG.Engine.TurnSteps;

namespace MTG.Engine.Gameplay;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly IGameDisplay _display;

    public GameEngine(GameContext context, IGameDisplay display)
    {
        _context = context;
        _display = display;
    }

    public async void StartGameLoop()
    {
        _context.ActivePlayer = _context.Players[0];
        _context.PriorityPlayer = _context.Players[0];

        _context.Initialize(_display);

        _context.TransitionTo(new UntapStep());

        while (!IsGameOver())
        {
            var action = await _context.PriorityPlayer.GetNextAction(_context);
            _context.HandleIncomingAction(action);
            CheckStateBasedActions();
        }
    }

    private bool IsGameOver() => _context.Players.Count(p => !p.IsEliminated) <= 1;

    private void CheckStateBasedActions()
    {
        foreach (var player in _context.Players.ToList())
        {
            if (player.LifeTotal <= 0 && !player.IsEliminated)
            {
                player.IsEliminated = true;
                _context.RemovePlayerFromGame(player);
                _display.LogGameEvent(new PlayerEliminationEvent(player.Name));
            }
        }
    }
}
