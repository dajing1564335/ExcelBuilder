using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
    public static T GetRandom<T>(this List<T> list) => list[Random.Range(0, list.Count)];
    public static T GetRandom<T>(this T[] array) => array[Random.Range(0, array.Length)];
    public static T GetRandom<T>(this ICollection<T> collection)
    {
        var index = Random.Range(0, collection.Count);
        using var enumerator = collection.GetEnumerator();
        for (var i = 0; i < index; i++)
        {
            enumerator.MoveNext();
        }
        return enumerator.Current;
    }

    public static List<T> Shuffle<T>(this List<T> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    public static T GetOrDefault<T>(this List<T> list, int count) => count >= 0 && count < list.Count ? list[count] : default;

    public static T TakeRandom<T>(this List<T> list)
    {
        var index = Random.Range(0, list.Count);
        var value = list[index];
        list.RemoveAt(index);
        return value;
    }

    public static void Swap<T>(this List<T> list, int index1, int index2)
    {
        (list[index2], list[index1]) = (list[index1], list[index2]);
    }
    public static void Swap<T>(this T[] array, int index1, int index2)
    {
        (array[index2], array[index1]) = (array[index1], array[index2]);
    }

    public static int RandomRateIndex(this List<int> rates)
    {
        var point = Random.Range(0, rates.Sum());
        var i = 0;
        for (; i < rates.Count && point >= rates[i]; point -= rates[i++]) ;
        return i;
    }
}
