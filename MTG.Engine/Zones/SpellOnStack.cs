using Microsoft.SqlServer.Management.XEvent;
using MTG.Core.Helper;
using MTG.Engine.Cards;
using MTG.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public class SpellOnStack : IStackObject
{
    public Guid Id { get; } = Guid.NewGuid();
    public CardInstance Card { get; }
    public CommanderPlayer Controller { get; }
    public IReadOnlyList<ITarget> Targets { get; }

    public string Name => throw new NotImplementedException();

    public SpellOnStack(CardInstance card, CommanderPlayer controller, List<ITarget> targets)
    {
        Card = card;
        Controller = controller;
        Targets = targets;
    }

    public Result Resolve(GameEngine game)
    {
        return Result.Success();
    }
}