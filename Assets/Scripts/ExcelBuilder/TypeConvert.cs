using UnityEngine;
using System.Collections.Generic;
using System;

public static class TypeConvert
{
    public static readonly List<string> SupportType = new List<string> { "int", "float", "bool", "string", "char", "MsgLabel" };

    public static T GetValue<T>(string value)
    {
        if (value == string.Empty)
        {
            return default;
        }

        if (typeof(T) == typeof(int))
        {
            try
            {
                return (T)(object)int.Parse(value);
            }
            catch
            {
                Debug.LogError($"{value} is not a int.");
            }
        }

        if (typeof(T) == typeof(float))
        {
            try
            {
                return (T)(object)float.Parse(value);
            }
            catch
            {
                Debug.LogError($"{value} is not a float.");
            }
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
            else
            {
                Debug.LogError($"{value} is not a char.");
            }
        }

        if (typeof(T).IsEnum)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch
            {
                Debug.LogError($"{value} is not a [{typeof(T).Name}].");
            }
        }

        return default;
    }

    public static int GetValue(string value, string[] types)
    {
        foreach (var type in types)
        {
            var ret = -1;
            var flag = true;
            try
            {
                ret = (int)Enum.Parse(Type.GetType($"Table.{type}"), value); 
            }
            catch
            {
                flag = false;
            }
            if (flag)
            {
                return ret;
            }
        }
        Debug.LogError($"{value} is not in [{types}].");
        return -1;
    }
}
