using System.Collections.Generic;
using UnityEngine;

public static class MsgAccessor
{
    private const string MsgDataPath = "Assets/ExcelData/Data/MsgData.asset";

    private static Dictionary<MsgLabel, string> _data;
        
    public static void LoadMsg(Language language)
    {
        var data = LoadManager.Instance.LoadAsset<MessageSO>("msg", MsgDataPath);
        LoadManager.Instance.UnloadAssetBundle("msg");
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
        _data ??= new Dictionary<MsgLabel, string>(UnityEditor.AssetDatabase.LoadAssetAtPath<MessageSO>(MsgDataPath).MsgDatas[0]);
#endif
        return _data[msgId];
    }
}
