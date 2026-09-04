using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Helper;

public static class NullGuard
{
    public static bool HasNullProperty<T>(T obj, out string? nullPropertyName)
    {
        if (obj == null)
        {
            nullPropertyName = typeof(T).Name;
            return true;
        }

        var nullProperty = typeof(T).GetProperties()
            .FirstOrDefault(prop => prop.GetValue(obj) == null);

        if (nullProperty != null)
        {
            nullPropertyName = nullProperty.Name;
            return true;
        }

        nullPropertyName = null;
        return false;
    }
}