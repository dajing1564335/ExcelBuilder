using System.Collections.Generic;
using UnityEngine;

public static class ListUtil
{
    public static T GetRandom<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T GetRandom<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}
