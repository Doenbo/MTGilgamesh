using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Enums;

public enum MarkerType //normally CounterType, but thats a c# thing
{
    PlusOnePlusOne,
    MinusOneMinusOne,
    Loyalty,
    Poison,
    Energy,
    Charge,
    Time,
    Shield
}
