using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T TakeRandom<T>(this List<T> list)
    {
        var index = Random.Range(0, list.Count);
        var value = list[index];
        list.RemoveAt(index);
        return value;
    }

    public static T GetRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    public static T GetRandom<T, V>(this Dictionary<T, V>.KeyCollection keys)
    {
        var index = Random.Range(0, keys.Count);
        var enumerator = keys.GetEnumerator();
        for (int i = 0; i < index; i++)
        {
            enumerator.MoveNext();
        }
        return enumerator.Current;
    }
}
