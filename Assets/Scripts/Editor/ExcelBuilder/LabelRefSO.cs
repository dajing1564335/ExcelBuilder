using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct LabelRefData
{
    public string Label;
    public int Id;

    public LabelRefData(string label, int id)
    {
        Label = label;
        Id = id;
    }
}

public class LabelRefSO : ScriptableObject
{
    public List<LabelRefData> Label = new List<LabelRefData>();
    public int MaxId = 0;

    public int AddLabel(string label)
    {
        var find = Label.Find(e => e.Label == label);
        if (find.Label == null)
        {
            Label.Add(new LabelRefData(label, MaxId));
            return MaxId++;
        }
        else
        {
            return find.Id;
        }
    }
}
