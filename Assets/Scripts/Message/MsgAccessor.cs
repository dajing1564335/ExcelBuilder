using System.Collections.Generic;
using UnityEngine;
using System;

public static class MsgAccessor
{   
    private static Dictionary<MsgLabel, string> _data;
        
    public static void LoadMsg(Language language)
    {
        var data = Resources.Load<MessageSO>("ExcelData/MsgData");
        if (!data)
        {
            Debug.LogError("No data! Please build data first.");
            return;
        }
        _data = new Dictionary<MsgLabel, string>();
        var datas = data.MsgDatas[(int)language].Datas;
        var msglabels = (MsgLabel[])Enum.GetValues(typeof(MsgLabel));
        for (int i = 0; i < datas.Length; i++)
        {
            _data.Add(msglabels[i], datas[i]);
        }
    }

    public static string GetMessage(MsgLabel msgId)
    {
#if UNITY_EDITOR
        if (_data == default)
        {
            LoadMsg(0);
        }
#endif
        return _data[msgId];
    }
}
