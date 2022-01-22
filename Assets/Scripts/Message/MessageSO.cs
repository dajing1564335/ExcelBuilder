using UnityEngine;
using System;
using System.Data;

public class MessageSO : ScriptableObject
{
    [Serializable]
    public struct MsgData
    {
        public string[] Datas;
    }

    public MsgData[] MsgDatas;

    public void CreateData(DataTableCollection tables, int labelCount)
    {
        MsgDatas = new MsgData[tables[0].Columns.Count - 1];
        for (int i = 0; i < MsgDatas.Length; i++)
        {
            MsgDatas[i].Datas = new string[labelCount];
        }

        int index = 0;
        foreach (DataTable table in tables)
        {
            for (int i = 1; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][0] is DBNull)
                {
                    continue;
                }
                for (int j = 1; j < table.Columns.Count; j++)
                {
                    MsgDatas[j - 1].Datas[index] = table.Rows[i][j].ToString();
                }
                index++;
            }
        }
    }
}
