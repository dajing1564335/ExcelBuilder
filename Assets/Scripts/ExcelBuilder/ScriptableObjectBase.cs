using UnityEngine;
using System.Data;

namespace Table
{ 
    public abstract class ScriptableObjectBase : ScriptableObject
    {
    #if UNITY_EDITOR
        public abstract void CreateData(DataTable table);
    #endif
    }
}