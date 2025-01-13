using UnityEngine;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;

public class MessageSO : ScriptableObject
{
    public SerializableDictionary<Language, SerializableDictionary<MsgLabel, string>> MsgDatas;

#if UNITY_EDITOR
    private const string PATTERN = @"\{MsgLabel\.([^\}]+)\}";

    public void Clear()
    {
        MsgDatas = new SerializableDictionary<Language, SerializableDictionary<MsgLabel, string>>();
        foreach (Language language in Enum.GetValues(typeof(Language)))
        {
            MsgDatas.Add(language, new SerializableDictionary<MsgLabel, string>());
        }
    }

    public void AddData(DataTableCollection tables)
    {
        foreach (DataTable table in tables)
        {
            for (int i = 1; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][0] is DBNull)
                {
                    continue;
                }
                MsgLabel msgLabel = (MsgLabel)Enum.Parse(typeof(MsgLabel), table.Rows[i][0].ToString());
                for (int j = 1; j < table.Columns.Count; j++)
                {
                    MsgDatas[(Language)(j - 1)].Add(msgLabel, table.Rows[i][j].ToString());
                }
            }
        }
    }

    //Replace {MsgLabel.***}
    public void ReplaceMessage()
    {
        foreach (var data in MsgDatas.Values)
        {
            foreach (var key in data.Keys)
            {
                data[key] = ReplaceMessage(data, key);
            }
        }
    }

    private string ReplaceMessage(SerializableDictionary<MsgLabel, string> data, MsgLabel key) 
        => Regex.Replace(data[key], PATTERN, match =>
        {
            var values = match.Groups[1].Value.Split(',');
            if (Enum.TryParse<MsgLabel>(values[0], out var label))
            {
                if (values.Length > 1)
                {
                    return string.Format(ReplaceMessage(data, label), values.Skip(1).Cast<object>().ToArray());
                }
                return ReplaceMessage(data, label);
            }
            Debug.LogWarning($"{values[0]} is not a MsgLabel in MsgLabel.{key} - {data[key]}");
            return data[key];
        });
#endif
}
