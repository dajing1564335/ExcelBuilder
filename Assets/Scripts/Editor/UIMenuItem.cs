using UnityEditor;
using UnityEngine;

public class UIMenuItem
{
    const string TextEx = "Prefabs/ExUI/TextEx";
    const string ButtonEx = "Prefabs/ExUI/ButtonEx";
    const string DropdownEx = "Prefabs/ExUI/DropdownEx";

    static Transform GetParent()
    {
        var parent = Selection.activeTransform;
        if (!parent)
        {
            parent = GameObject.FindObjectOfType<Canvas>()?.transform;
        }
        return parent;
    }

    [MenuItem("GameObject/UI/Text - Ex", false, 2001)]
    static void CreateTextEx()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(TextEx), GetParent());
        obj.name = "TextEx";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }

    [MenuItem("GameObject/UI/Button - Ex", false, 2031)]
    static void CreateButtonEx()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(ButtonEx), GetParent());
        obj.name = "ButtonEx";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }

    [MenuItem("GameObject/UI/Dropdown - Ex", false, 2036)]
    static void CreateDropdownEx()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(DropdownEx), GetParent());
        obj.name = "DropdownEx";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }

}
