#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;

public static class TypeConvert
{
    public static readonly Dictionary<string, int> SupportType = new()
    {
        ["int"] = 1,
        ["float"] = 1,
        ["bool"] = 1,
        ["string"] = 1,
        ["char"] = 1,
        ["MsgLabel"] = 1,
        ["Vector3"] = 3,
        ["Vector3Int"] = 3,
        ["Vector2"] = 2,
        ["Vector2Int"] = 2,
    };

    public static T GetValue<T>(object valueObj)
    {
        if (valueObj is DBNull)
        {
            return default;
        }
        var value = valueObj.ToString();
        if (value == string.Empty)
        {
            return default;
        }

        if (typeof(T) == typeof(int))
        {
            if (int.TryParse(value, out int retValue))
            {
                return (T)(object)retValue;
            }
            Debug.LogError($"{value} is not a int.");
        }

        if (typeof(T) == typeof(float))
        {
            if (float.TryParse(value, out float retValue))
            {
                return (T)(object)retValue;
            }
            Debug.LogError($"{value} is not a float.");
        }

        if (typeof(T) == typeof(bool))
        {
            if (value == "1" || value == "true")
            {
                return (T)(object)true;
            }
            if (value == "0" || value == "false")
            {
                return (T)(object)false;
            }
            Debug.LogError($"{value} is not a bool.");
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)value;
        }

        if (typeof(T) == typeof(char))
        {
            if (value.Length == 1)
            {
                return (T)(object)value[0];
            }
            Debug.LogError($"{value} is not a char.");
        }

        if (typeof(T).IsEnum)
        {
            if (Enum.TryParse(typeof(T), value, out object retValue))
            {
                return (T)retValue;
            }
            Debug.LogError($"{value} is not a [{typeof(T).Name}].");
        }

        return default;
    }

    public static T GetValue<T>(object value1, object value2)
    {
        if (typeof(T) == typeof(Vector2))
        {
            return (T)(object)new Vector2(GetValue<float>(value1), GetValue<float>(value2));
        }

        if (typeof(T) == typeof(Vector2Int))
        {
            return (T)(object)new Vector2Int(GetValue<int>(value1), GetValue<int>(value2));
        }

        return default;
    }

    public static T GetValue<T>(object value1, object value2, object value3)
    {
        if (typeof(T) == typeof(Vector3))
        {
            return (T)(object)new Vector3(GetValue<float>(value1), GetValue<float>(value2), GetValue<float>(value3));
        }

        if (typeof(T) == typeof(Vector3Int))
        {
            return (T)(object)new Vector3Int(GetValue<int>(value1), GetValue<int>(value2), GetValue<int>(value3));
        }

        return default;
    }

    public static int GetValue(object valueObj, string types)
    {
        if (valueObj is DBNull)
        {
            return default;
        }
        var value = valueObj.ToString();
        if (value == string.Empty)
        {
            return default;
        }

        if (int.TryParse(value, out int intValue))
        {
            return intValue;
        }

        var index = value.IndexOf('.');
        if (index == -1)
        {
            foreach (var type in types.Split(";"))
            {
                var t = Type.GetType($"Table.{type}");
                if (t != null && Enum.TryParse(t, value, out object retValue))
                {
                    return (int)retValue;
                }
                t = Type.GetType(type);
                if (t != null && Enum.TryParse(t, value, out retValue))
                {
                    return (int)retValue;
                }
            }
            Debug.LogError($"[{value}] is not in [{types}]");
        }
        else
        {
            var type = value[..index];
            var newValue = value[(index + 1)..];
            var t = Type.GetType($"Table.{type}");
            if (t != null && Enum.TryParse(t, newValue, out object retValue))
            {
                return (int)retValue;
            }
            t = Type.GetType(type);
            if (t != null && Enum.TryParse(t, newValue, out retValue))
            {
                return (int)retValue;
            }
            Debug.LogError($"[{newValue}] is not in [{type}]");
        }

        return default;
    }
}
#endif