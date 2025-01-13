#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;

public static class TypeConvert
{
    public static readonly List<string> SupportType = new()
    {
        "int",
        "float",
        "bool",
        "string",
        "char",
        "MsgLabel",
        "Vector3",
        "Vector3Int",
        "Vector2",
        "Vector2Int"
    };
    //Support type[]
    public static readonly List<string> SupportListType = new()
    {
        "int",
        "float",
        "bool",
        "string",
        "char",
        "MsgLabel",
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

        var type = typeof(T);
        if (type == typeof(int))
        {
            if (int.TryParse(value, out int retValue))
            {
                return (T)(object)retValue;
            }
            Debug.LogError($"{value} is not a int.");
        }

        if (type == typeof(float))
        {
            if (float.TryParse(value, out float retValue))
            {
                return (T)(object)retValue;
            }
            Debug.LogError($"{value} is not a float.");
        }

        if (type == typeof(bool))
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

        if (type == typeof(string))
        {
            return (T)(object)value;
        }

        if (type == typeof(char))
        {
            if (value.Length == 1)
            {
                return (T)(object)value[0];
            }
            Debug.LogError($"{value} is not a char.");
        }
        
        if (type == typeof(Vector3))
        {
            var values = value.Split(',');
            if (values.Length == 3
                && float.TryParse(values[0], out var x)
                && float.TryParse(values[1], out var y)
                && float.TryParse(values[2], out var z))
            {
                return (T)(object)new Vector3(x, y, z);
            }
            Debug.LogError($"{value} is not a Vector3.");
        }
        
        if (type == typeof(Vector3Int))
        {
            var values = value.Split(',');
            if (values.Length == 3
                && int.TryParse(values[0], out var x)
                && int.TryParse(values[1], out var y)
                && int.TryParse(values[2], out var z))
            {
                return (T)(object)new Vector3Int(x, y, z);
            }
            Debug.LogError($"{value} is not a Vector3Int.");
        }
        
        if (type == typeof(Vector2))
        {
            var values = value.Split(',');
            if (values.Length == 2
                && float.TryParse(values[0], out var x)
                && float.TryParse(values[1], out var y))
            {
                return (T)(object)new Vector2(x, y);
            }
            Debug.LogError($"{value} is not a Vector2.");
        }
        
        if (type == typeof(Vector2Int))
        {
            var values = value.Split(',');
            if (values.Length == 2
                && int.TryParse(values[0], out var x)
                && int.TryParse(values[1], out var y))
            {
                return (T)(object)new Vector2Int(x, y);
            }
            Debug.LogError($"{value} is not a Vector2Int.");
        }

        if (type.IsEnum)
        {
            if (Enum.TryParse(type, value, out object retValue))
            {
                return (T)retValue;
            }
            Debug.LogError($"{value} is not a [{type.Name}].");
        }

        if (type == typeof(List<int>))
        {
            var retValue = new List<int>();
            foreach (var v in value.Split(','))
            {
                retValue.Add(GetValue<int>(v));
            }
            return (T)(object)retValue;
        }

        if (type == typeof(List<float>))
        {
            var retValue = new List<float>();
            foreach (var v in value.Split(','))
            {
                retValue.Add(GetValue<float>(v));
            }
            return (T)(object)retValue;
        }

        if (type == typeof(List<bool>))
        {
            var retValue = new List<bool>();
            foreach (var v in value.Split(','))
            {
                retValue.Add(GetValue<bool>(v));
            }
            return (T)(object)retValue;
        }

        if (type == typeof(List<string>))
        {
            var retValue = new List<string>();
            foreach (var v in value.Split(','))
            {
                retValue.Add(GetValue<string>(v));
            }
            return (T)(object)retValue;
        }

        if (type == typeof(List<char>))
        {
            var retValue = new List<char>();
            foreach (var v in value.Split(','))
            {
                retValue.Add(GetValue<char>(v));
            }
            return (T)(object)retValue;
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

    public static List<int> GetListValue(object valueObj, string types)
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
        var retValue = new List<int>();
        foreach (var v in value.Split(','))
        {
            retValue.Add(GetValue(v, types));
        }
        return retValue;
    }
}
#endif