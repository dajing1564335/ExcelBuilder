using System;
using System.Collections.Generic;
using UnityEngine;

public class TableAccessorBase<T, V>
{
    private Dictionary<int, T> _data;

    public TableAccessorBase()
    {
        var data = Resources.Load<ScriptableObjectBase>($"ExcelData/{typeof(T).Name}");
        if (!data)
        {
            Debug.LogError("No data! Please build data first.");
            return;
        }
        _data = new Dictionary<int, T>();
        var datas = (List<T>)data.GetType().GetField("Datas").GetValue(data);
        var msglabels = (int[])Enum.GetValues(typeof(V));
        for (int i = 0; i < datas.Count; i++)
        {
            _data.Add(msglabels[i], datas[i]);
        }
    }

    public T this[int index] => _data[index];

    public T this[V label] => _data[(int)(object)label];

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var data in _data.Values)
        {
            yield return data;
        }
    }
}
