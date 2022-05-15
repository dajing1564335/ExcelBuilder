using UnityEngine;

public class LabelRefSO : ScriptableObject
{
    public SerializableDictionary<string, int> LabelRef = new SerializableDictionary<string, int>();
    public int MaxId = 0;

    public int GetId(string label)
    {
        if (LabelRef.TryGetValue(label, out var id))
        {
            return id;
        }
        else
        {
            LabelRef.Add(label, MaxId);
            return MaxId++;
        }
    }
}
