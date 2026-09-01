using MTG.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Abilities;

public static class CardContextExtensions
{
    public static bool HasCardType(this CardContext context, CardType type)
    {
        return context.CardTypes.Contains(type);
    }
    public static bool HasSubtype<TEnum>(this CardContext context, TEnum subtype) where TEnum : struct, Enum
    {
        return context.Subtypes.OfType<TEnum>().Contains(subtype);
    }
    public static IEnumerable<TEnum> GetSubtypes<TEnum>(this CardContext context) where TEnum : struct, Enum
    {
        return context.Subtypes.OfType<TEnum>();
    }
}