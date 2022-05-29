using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(DropdownEx))]
public class DropdownExEditor : Editor
{
    static readonly Array MsgIdArray = Enum.GetValues(typeof(MsgLabel));

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            return;
        }

        var dropdownEx = (DropdownEx)target;
        var labels = serializedObject.FindProperty("_labels");
        List<string> strings = new();
        for (int i = 0; i < labels.arraySize; ++i)
        {
            strings.Add(MsgAccessor.GetMessage((MsgLabel)MsgIdArray.GetValue(labels.GetArrayElementAtIndex(i).enumValueIndex)));
        }
        dropdownEx.ClearOptions();
        dropdownEx.AddOptions(strings);
        dropdownEx.captionText.text = strings.Count > 0 ? strings[0] : string.Empty;
    }
}
