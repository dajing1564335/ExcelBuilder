using System.Collections.Generic;
using UnityEngine;

public static class MsgAccessor
{
    private const string MsgDataPath = "ExcelData/Data/MsgData";

    private static Dictionary<MsgLabel, string> _data;
        
    public static void LoadMsg(Language language)
    {
        var data = Resources.Load<MessageSO>(MsgDataPath);
        if (!data)
        {
            Debug.LogError("No data! Please build data first.");
            return;
        }
        _data = new Dictionary<MsgLabel, string>(data.MsgDatas[language]);
        Resources.UnloadAsset(data);
    }

    public static string GetMessage(MsgLabel msgId)
    {
#if UNITY_EDITOR
        if (_data == null)
        {
            var data = Resources.Load<MessageSO>(MsgDataPath);
            _data = new Dictionary<MsgLabel, string>(data.MsgDatas[0]);
            Resources.UnloadAsset(data);
        }
#endif
        return _data[msgId];
    }
}
