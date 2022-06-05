using System.Collections.Generic;
using UnityEngine;

namespace AA
{
    public enum ABC
    {
        AA,
        BB,
        CC,
    }
}

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
        _data = new Dictionary<MsgLabel, string>(data.MsgDatas[language]);
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
