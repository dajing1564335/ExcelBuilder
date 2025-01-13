using System.Collections.Generic;
using System.Linq;
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

    public static string GetMessage(MsgLabel msgId, List<int> param)
        => param == null ? GetMessage(msgId) : GetMessage(msgId, param.Cast<object>().ToArray());

    public static string GetMessage(MsgLabel msgId, params object[] param)
    {
#if UNITY_EDITOR
        if (_data == null)
        {
            LoadMsg(0);
        }
#endif
        return param.Length == 0 ? _data[msgId] : string.Format(_data[msgId], param);
    }
}