using System.Collections.Generic;
using UnityEngine;

public class TableAccessorList<T>
{
    private List<T> _data;

    public TableAccessorList()
    {
        var data = Resources.Load<ScriptableObjectBase>($"ExcelData/{typeof(T).Name}");
        if (!data)
        {
            Debug.LogError("No data! Please build data first.");
            return;
        }
        _data = (List<T>)data.GetType().GetField("Datas").GetValue(data);
    }

    public T this[int index] => _data[index];

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var data in _data)
        {
            yield return data;
        }
    }
}
