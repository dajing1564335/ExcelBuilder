using System.Collections.Generic;
using UnityEngine;

public class TableAccessorList<T, V>
{
    private readonly List<V> _data;

    public TableAccessorList()
    {
        var data = LoadManager.Instance.LoadAsset<ScriptableObjectBase>("table", $"Assets/ExcelData/Data/{typeof(V).Name}.asset");
        if (!data)
        {
            Debug.LogError("No data! Please build data first.");
            return;
        }
        _data = new List<V>((List<V>)data.GetType().GetField("Datas").GetValue(data));
    }

    public V this[int index] => _data[index];

    public V this[T label] => _data[(int)(object)label];

    public int Count => _data.Count;

    public IEnumerator<V> GetEnumerator()
    {
        foreach (var data in _data)
        {
            yield return data;
        }
    }
}
