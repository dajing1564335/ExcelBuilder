using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class TableAccessorList<T, V>
    {
        private readonly List<V> _data;

        public TableAccessorList()
        {
            var data = Resources.Load<ScriptableObjectBase>($"ExcelData/Data/{typeof(V).Name}");
            if (!data)
            {
                Debug.LogError("No data! Please build data first.");
                return;
            }
            _data = new List<V>((List<V>)data.GetType().GetField("Datas").GetValue(data));
            Resources.UnloadAsset(data);
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
}