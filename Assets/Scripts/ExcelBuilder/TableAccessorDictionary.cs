using System.Collections.Generic;
using UnityEngine;

public class TableAccessorDictionary<T, V>
{
    private readonly Dictionary<T, V> _data;

    public TableAccessorDictionary()
    {
        var data = LoadManager.Instance.LoadAsset<ScriptableObjectBase>("table", $"Assets/ExcelData/Data/{typeof(V).Name}.asset");
        if (!data)
        {
            Debug.LogError("No data! Please build data first.");
            return;
        }
        _data = new Dictionary<T, V>((SerializableDictionary<T, V>)data.GetType().GetField("Datas").GetValue(data));
    }

    public V this[int index] => _data[(T)(object)index];

    public V this[T label] => _data[label];

    public int Count => _data.Count;

    public IEnumerator<V> GetEnumerator()
    {
        foreach (var data in _data.Values)
        {
            yield return data;
        }
    }
}
