using UnityEngine;
using System.Collections.Generic;
using System;

public static class TypeConvert
{
    public static readonly List<string> SupportType = new() { "int", "float", "bool", "string", "char", "MsgLabel" };

    public static T GetValue<T>(string value)
    {
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

    public static int GetValue(string value, string types)
    {
        if (value == string.Empty)
        {
            return default;
        }

        foreach (var type in types.Split(";"))
        {
            if (Enum.TryParse(Type.GetType($"Table.{type}Enum"), value, out object retValue))
            {
                return (int)retValue;
            }
        }
        Debug.LogError($"[{value}] is not in [{types}]");
        return default;
    }
}
