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
        var array = GetEnumArray<T>();
        return array[UnityEngine.Random.Range(0, array.Length)];
    }
}
