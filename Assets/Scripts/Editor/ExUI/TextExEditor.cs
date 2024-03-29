using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(TextEx))]
public class TextExEditor : TextEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_label"));
        var textEx = (TextEx)target;
        textEx.text = MsgAccessor.GetMessage(textEx.Label);
        base.OnInspectorGUI();
    }
}

