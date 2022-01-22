using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(DropdownEx))]
public class DropdownExEditor : Editor
{
    static Array MsgIdArray = Enum.GetValues(typeof(MsgLabel));

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            return;
        }

        var dropdownEx = (DropdownEx)target;
        var msgIds = serializedObject.FindProperty("_msgIds");
        List<string> strings = new List<string>();
        for(int i = 0; i < msgIds.arraySize; ++i)
        {
            strings.Add(MsgAccessor.GetMessage((MsgLabel)MsgIdArray.GetValue(msgIds.GetArrayElementAtIndex(i).enumValueIndex)));
        }
        dropdownEx.ClearOptions();
        dropdownEx.AddOptions(strings);
        dropdownEx.captionText.text = strings.Count > 0 ? strings[0] : string.Empty;
    }
}
