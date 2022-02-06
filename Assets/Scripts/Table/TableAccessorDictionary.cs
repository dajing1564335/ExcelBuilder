using System;
using System.Collections.Generic;
using UnityEngine;

public class TableAccessorDictionary<T>
{
    private Dictionary<int, T> _data;

    public TableAccessorDictionary()
    {
        var data = Resources.Load<ScriptableObjectBase>($"ExcelData/{typeof(T).Name}");
        if (!data)
        {
            Debug.LogError("No data! Please build data first.");
            return;
        }
        _data = new Dictionary<int, T>();
        var datas = (List<T>)data.GetType().GetField("Datas").GetValue(data);
        var msglabels = (int[])Enum.GetValues(typeof(T));
        for (int i = 0; i < datas.Count; i++)
        {
            _data.Add(msglabels[i], datas[i]);
        }
    }

    public T this[int label] => _data[label];
}
