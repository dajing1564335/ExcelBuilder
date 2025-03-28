using UnityEngine;

namespace Table
{
    public class LabelRefSO : ScriptableObject
    {
        public SerializableDictionary<string, int> LabelRef = new();
        public int MaxId = 0;

        public int GetId(string label)
        {
            if (LabelRef.TryGetValue(label, out var id))
            {
                return id;
            }
            LabelRef.Add(label, MaxId);
            return MaxId++;
        }
    }
}