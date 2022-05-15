using UnityEngine;
using System;
using System.Data;

public class MessageSO : ScriptableObject
{
    public SerializableDictionary<Language, SerializableDictionary<MsgLabel, string>> MsgDatas;

    public void CreateData(DataTableCollection tables)
    {
        MsgDatas = new SerializableDictionary<Language, SerializableDictionary<MsgLabel, string>>();
        foreach (Language language in Enum.GetValues(typeof(Language)))
        {
            MsgDatas.Add(language, new SerializableDictionary<MsgLabel, string>());
        }

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
}
