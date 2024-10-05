using System.Collections.Generic;
using UnityEngine;
using System;

public static class EnumUtil
{
    static readonly Dictionary<Type, Array> _enumArrays = new();

    public static T[] GetEnumArray<T>() where T : struct
    {
        var type = typeof(T);
        if (!type.IsEnum)
        {
            Debug.LogWarning($"{type.Name} is not a Enum.");
            return default;
        }
        if (!_enumArrays.TryGetValue(type, out var ret))
        {
            ret = Enum.GetValues(type);
            _enumArrays.Add(type, ret);
        }
        return (T[])ret;
    }

    public static T GetRandom<T>() where T : struct
    {
        return GetEnumArray<T>().GetRandom();
    }

    public static void Release()
    {
        _enumArrays.Clear();
    }
}
