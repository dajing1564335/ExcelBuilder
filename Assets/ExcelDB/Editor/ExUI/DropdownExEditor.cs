using System.Collections.Generic;
using UnityEditor;

namespace TMPro.EditorUtilities
{
    [CustomEditor(typeof(DropdownEx))]
    public class DropdownExEditor : DropdownEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_labels"));
            var dropdownEx = (DropdownEx)target;
            List<string> strings = new();
            foreach (var label in dropdownEx.Labels)
            {
                strings.Add(MsgAccessor.GetMessage(label));
            }
            dropdownEx.ClearOptions();
            dropdownEx.AddOptions(strings);
            dropdownEx.captionText.text = strings.Count > 0 ? strings[0] : string.Empty;
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}