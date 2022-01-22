using System;
using System.Collections.Generic;
using UnityEngine;

public class TableAccessorBase<T, V>
{
    private Dictionary<T, V> _data;

    public TableAccessorBase()
    {
        var data = Resources.Load<ScriptableObjectBase>($"ExcelData/{typeof(V).Name}");
        if (!data)
        {
            Debug.LogError("No data! Please build data first.");
            return;
        }
        _data = new Dictionary<T, V>();
        var datas = (List<V>)data.GetType().GetField("Datas").GetValue(data);
        var msglabels = (T[])Enum.GetValues(typeof(T));
        for (int i = 0; i < datas.Count; i++)
        {
            _data.Add(msglabels[i], datas[i]);
        }
    }

    public V this[T label] => _data[label];
}
