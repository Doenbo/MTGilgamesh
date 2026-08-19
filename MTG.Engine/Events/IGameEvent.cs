using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Events;

public interface IGameEvent
{
    DateTime Timestamp { get; }
    string Description { get; }
}
