using UnityEditor;
using UnityEngine;

public class UIMenuItem
{
    const string TextEx = "Assets/Prefabs/ExUI/TextEx.prefab";
    const string ButtonEx = "Assets/Prefabs/ExUI/ButtonEx.prefab";
    const string DropdownEx = "Assets/Prefabs/ExUI/DropdownEx.prefab";

    [MenuItem("GameObject/UI/Text - Ex", false, 2001)]
    static void CreateTextEx()
    {
        GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(TextEx), Selection.activeTransform);
        obj.name = "TextEx";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }

    [MenuItem("GameObject/UI/Button - Ex", false, 2031)]
    static void CreateButtonEx()
    {
        GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(ButtonEx), Selection.activeTransform);
        obj.name = "ButtonEx";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }

    [MenuItem("GameObject/UI/Dropdown - Ex", false, 2036)]
    static void CreateDropdownEx()
    {
        GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(DropdownEx), Selection.activeTransform);
        obj.name = "DropdownEx";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }
}
