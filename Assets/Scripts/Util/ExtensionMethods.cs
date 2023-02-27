using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    public static T GetRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    public static T GetRandom<T>(this ICollection<T> collection)
    {
        var index = Random.Range(0, collection.Count);
        var enumerator = collection.GetEnumerator();
        for (int i = 0; i < index; i++)
        {
            enumerator.MoveNext();
        }
        return enumerator.Current;
    }

    public static T TakeRandom<T>(this List<T> list)
    {
        var index = Random.Range(0, list.Count);
        var value = list[index];
        list.RemoveAt(index);
        return value;
    }

    public static void Swap<T>(this List<T> list, int index1, int index2)
    {
        var temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }
    public static void Swap<T>(this T[] array, int index1, int index2)
    {
        var temp = array[index1];
        array[index1] = array[index2];
        array[index2] = temp;
    }
}
