using MTG.Core.Helper;
using MTG.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public interface IStackObject
{
    Guid Id { get; }
    CommanderPlayer Controller { get; }
    string Name { get; }

    Result Resolve(GameEngine engine);
}