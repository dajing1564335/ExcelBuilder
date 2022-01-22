using UnityEditor;

[CustomEditor(typeof(TextEx))]
public class TextExEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var textEx = (TextEx)target;
        textEx.text = MsgAccessor.GetMessage(textEx.MsgId);
    }
}

