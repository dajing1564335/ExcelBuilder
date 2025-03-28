using UnityEditor;

namespace TMPro.EditorUtilities
{
    [CustomEditor(typeof(TextMeshProUGUIEx))]
    public class TextMeshProUGUIExEditor : TMP_EditorPanelUI
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_label"));
            var textEx = (TextMeshProUGUIEx)target;
            textEx.text = MsgAccessor.GetMessage(textEx.Label);
            base.OnInspectorGUI();
        }
    }
}
