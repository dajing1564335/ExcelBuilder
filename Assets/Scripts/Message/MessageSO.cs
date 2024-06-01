using UnityEngine;
using System;

public class MessageSO : ScriptableObject
{
    public SerializableDictionary<Language, SerializableDictionary<MsgLabel, string>> MsgDatas;

#if UNITY_EDITOR
    public void Clear()
    {
        MsgDatas = new SerializableDictionary<Language, SerializableDictionary<MsgLabel, string>>();
        foreach (Language language in Enum.GetValues(typeof(Language)))
        {
            MsgDatas.Add(language, new SerializableDictionary<MsgLabel, string>());
        }
    }

    public void AddData(Table.CsvDataTable table)
    {
        for (int i = 1; i < table.Rows.Count; i++)
        {
            if (table.Rows[i][0] == string.Empty)
            {
                continue;
            }
            MsgLabel msgLabel = (MsgLabel)Enum.Parse(typeof(MsgLabel), table.Rows[i][0]);
            for (int j = 1; j < table.Columns.Count; j++)
            {
                MsgDatas[(Language)(j - 1)].Add(msgLabel, table.Rows[i][j]);
            }
        }
    }
#endif
}
