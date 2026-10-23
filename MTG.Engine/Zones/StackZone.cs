using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Engine.Cards;
using MTG.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public class StackZone
{
    private readonly List<IStackObject> _objects = [];

    public ZoneType Type => ZoneType.Stack;
    public bool IsPublic => true;
    public bool IsEmpty => _objects.Count == 0;
    public int Count => _objects.Count;

    public IReadOnlyList<IStackObject> Objects => _objects.AsReadOnly();

    // Stack Operations
    public Result Push(IStackObject item)
    {
        if (item is null)
            return Result.Failure("Cannot push a null object to the stack.");

        _objects.Add(item);
        return Result.Success();
    }

    public Result<IStackObject> Pop()
    {
        if (_objects.Count == 0)
            return Result<IStackObject>.Failure("The stack is empty.");

        var top = _objects[^1];
        _objects.RemoveAt(_objects.Count - 1);
        return Result<IStackObject>.Success(top);
    }

    public Result<IStackObject> Peek()
    {
        if (_objects.Count == 0)
            return Result<IStackObject>.Failure("The stack is empty.");

        return Result<IStackObject>.Success(_objects[^1]);
    }
}