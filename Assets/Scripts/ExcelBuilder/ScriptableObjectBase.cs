using UnityEngine;
using System.Collections.Generic;

namespace Table
{
#if UNITY_EDITOR
    public class CsvDataTable
    {
        public string TableName;
        public List<List<string>> Rows;

        public List<string> Columns => Rows[0];
    }
#endif

    public abstract class ScriptableObjectBase : ScriptableObject
    {
#if UNITY_EDITOR
        public abstract void CreateData(CsvDataTable table);
#endif
    }
}