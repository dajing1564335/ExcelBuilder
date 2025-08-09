#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

namespace Table
{
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

            // 检查是否为泛型类型，并且是 List<Enum>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var itemType = type.GetGenericArguments()[0];
                // 确保泛型参数是枚举类型
                if (itemType.IsEnum)
                {
                    // 创建 List<T> 的实例
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    var retValue = Activator.CreateInstance(listType);
                    // 使用反射向列表中添加元素
                    var addMethod = listType.GetMethod("Add");
                    foreach (var v in value.Split(','))
                    {
                        if (Enum.TryParse(itemType, v, out object enumValue))
                        {
                            addMethod.Invoke(retValue, new[] { enumValue });
                        }
                        else
                        {
                            Debug.LogError($"{v} is not a [{itemType}] in [{value}].");
                        }
                    }
                    return (T)retValue;
                }
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
                    var t = Type.GetType($"Table.DB{type}");
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
                var t = Type.GetType($"Table.DB{type}");
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
            foreach (var v in Replace(value).Split(','))
            {
                retValue.Add(GetValue(v, types));
            }
            return retValue;
        }

        private static string Replace(string str)
        {
            var result = new StringBuilder();
            var brackets = new Stack<char>();
            var isNum = false;
            str = str.Replace(" ", string.Empty);
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (brackets.Count > 0 && brackets.Peek() == '{')
                {
                    if (c is >= '0' and <= '9')
                    {
                        if (!isNum)
                        {
                            result.Append(",");
                            isNum = true;
                        }
                    }
                    else
                    {
                        isNum = false;
                    }
                    switch (c)
                    {
                        case '{':
                            result.Append("," + (int)SpecialParam.Expression);
                            brackets.Push(c);
                            break;
                        case '}':
                            result.Append("," + (int)SpecialParam.End);
                            brackets.Pop();
                            break;
                        case '[':
                            result.Append(',');
                            brackets.Push(c);
                            break;
                        case ']':
                            Debug.LogError("Miss '[' in -> " + str);
                            break;
                        case '(':
                            result.Append("," + (int)Operator.Left);
                            break;
                        case ')':
                            result.Append("," + (int)Operator.Right);
                            break;
                        case '+':
                            result.Append("," + (int)Operator.Add);
                            break;
                        case '-':
                            result.Append("," + (int)Operator.Sub);
                            break;
                        case '*':
                            result.Append("," + (int)Operator.Mul);
                            break;
                        case '/':
                            result.Append("," + (int)Operator.Div);
                            break;
                        case '%':
                            result.Append("," + (int)Operator.Mod);
                            break;
                        case '<':
                            if (i < str.Length - 1 && str[i + 1] == '=')
                            {
                                result.Append("," + (int)Operator.LessEqual);
                                i++;
                            }
                            else
                            {
                                result.Append("," + (int)Operator.Less);
                            }
                            break;
                        case '>':
                            if (i < str.Length - 1 && str[i + 1] == '=')
                            {
                                result.Append("," + (int)Operator.GreaterEqual);
                                i++;
                            }
                            else
                            {
                                result.Append("," + (int)Operator.Greater);
                            }
                            break;
                        case '=':
                            if (i < str.Length - 1 && str[i + 1] == '=')
                            {
                                result.Append("," + (int)Operator.Equal);
                                i++;
                            }
                            break;
                        case '!':
                            if (i < str.Length - 1 && str[i + 1] == '=')
                            {
                                result.Append("," + (int)Operator.NotEqual);
                                i++;
                            }
                            else
                            {
                                result.Append("," + (int)Operator.Not);
                            }
                            break;
                        case '&':
                            if (i < str.Length - 1 && str[i + 1] == '&')
                            {
                                result.Append("," + (int)Operator.And);
                                i++;
                            }
                            else
                            {
                                Debug.LogError("Wrong operator. -> '&'");
                            }
                            break;
                        case '|':
                            if (i < str.Length - 1 && str[i + 1] == '|')
                            {
                                result.Append("," + (int)Operator.Or);
                                i++;
                            }
                            else
                            {
                                Debug.LogError("Wrong operator. -> '|'");
                            }
                            break;
                        default:
                            result.Append(c);
                            break;
                    }
                }
                else
                {
                    var inMiddle = brackets.Count > 0 && brackets.Peek() == '[';
                    switch (c)
                    {
                        case '{':
                            if (inMiddle) Debug.LogError("Miss ']' in -> " + str);
                            else
                            {
                                result.Append((int)SpecialParam.Expression);
                                brackets.Push(c);
                            }
                            break;
                        case '}':
                            if (inMiddle) Debug.LogError("Miss ']' in -> " + str);
                            else Debug.LogError("Miss '{' in -> " + str);
                            break;
                        case '[':
                            brackets.Push(c);
                            break;
                        case ']':
                            if (inMiddle) brackets.Pop();
                            else Debug.LogError("Miss '[' in -> " + str);
                            break;
                        default:
                            result.Append(c);
                            break;
                    }
                }
            }
            return result.ToString();
        }
    }
}
#endif