using UnityEditor;
using UnityEngine;

public class UIMenuItem
{
    //const string TextEx = "Assets/Prefabs/ExUI/TextEx.prefab";
    //const string TextMeshProUGUIEx = "Assets/Prefabs/ExUI/TextEx (TMP).prefab";
    //const string ButtonEx = "Assets/Prefabs/ExUI/ButtonEx.prefab";
    //const string ButtonExTMP = "Assets/Prefabs/ExUI/ButtonEx (TMP).prefab";
    //const string DropdownEx = "Assets/Prefabs/ExUI/DropdownEx.prefab";
    //const string DropdownExTMP = "Assets/Prefabs/ExUI/DropdownEx (TMP).prefab";
    
    const string TextMeshProUGUIExGUID = "985ce8671b4188341927b12e48b06319";
    const string ButtonExTMPGUID = "d765ce726e18c3f4781d833f714d40fe";
    const string DropdownExTMPGUID = "faa4720bafefeeb40b7fc62f6d9c6731";

    //[MenuItem("GameObject/UI/Text - Ex", false, 2001)]
    //static void CreateTextEx()
    //{
    //    GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(TextEx), Selection.activeTransform);
    //    obj.name = "TextEx";
    //    obj.transform.localPosition = Vector3.zero;
    //    Selection.activeGameObject = obj;
    //}

    [MenuItem("GameObject/UI/Text - TextMeshPro - Ex", false, 2001)]
    static void CreateTextMeshProUGUIEx()
    {
        //GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(TextMeshProUGUIEx), Selection.activeTransform);
        GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(TextMeshProUGUIExGUID)), Selection.activeTransform);
        obj.name = "TextEx (TMP)";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }

    //[MenuItem("GameObject/UI/Button - Ex", false, 2031)]
    //static void CreateButtonEx()
    //{
    //    GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(ButtonEx), Selection.activeTransform);
    //    obj.name = "ButtonEx";
    //    obj.transform.localPosition = Vector3.zero;
    //    Selection.activeGameObject = obj;
    //}

    [MenuItem("GameObject/UI/Button - TextMeshPro - Ex", false, 2031)]
    static void CreateButtonExTMP()
    {
        //GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(ButtonExTMP), Selection.activeTransform);
        GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ButtonExTMPGUID)), Selection.activeTransform);
        obj.name = "ButtonEx";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }

    //[MenuItem("GameObject/UI/Dropdown - Ex", false, 2036)]
    //static void CreateDropdownEx()
    //{
    //    GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(DropdownEx), Selection.activeTransform);
    //    obj.name = "DropdownEx";
    //    obj.transform.localPosition = Vector3.zero;
    //    Selection.activeGameObject = obj;
    //}

    [MenuItem("GameObject/UI/Dropdown - TextMeshPro - Ex", false, 2036)]
    static void CreateDropdownExTMP()
    {
        //GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(DropdownExTMP), Selection.activeTransform);
        GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(DropdownExTMPGUID)), Selection.activeTransform);
        obj.name = "DropdownEx";
        obj.transform.localPosition = Vector3.zero;
        Selection.activeGameObject = obj;
    }
}
