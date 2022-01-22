using UnityEngine;
using System.Data;

public abstract class ScriptableObjectBase : ScriptableObject
{

    public abstract void CreateData(DataTable table);
}
