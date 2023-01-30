using UnityEngine;
using System.Data;

public abstract class ScriptableObjectBase : ScriptableObject
{
#if UNITY_EDITOR
    public abstract void CreateData(DataTable table);
#endif
}
