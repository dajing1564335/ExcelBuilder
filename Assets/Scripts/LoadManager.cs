using System.Collections.Generic;
using UnityEngine;

public class LoadManager : SingletonMonoBehaviour<LoadManager>
{
    public static readonly string StreamFolderWindows = Application.streamingAssetsPath + "/Windows/";

#if UNITY_EDITOR
    [SerializeField]
    private bool LoadFromAssetBundle;
    [SerializeField]
    private SerializableDictionary<string, AssetBundle> _assetBundles = new();
#else
    private readonly Dictionary<string, AssetBundle> _assetBundles = new();
#endif

    private AssetBundle LoadAssetBundle(string name)
    {
        if (!_assetBundles.TryGetValue(name, out var ab))
        {
            ab = AssetBundle.LoadFromFile(StreamFolderWindows + name);
            if (ab)
            {
                _assetBundles.Add(name, ab);
            }
            else
            {
                Debug.LogError($"Load AssetBundle Error. --{name}");
            }
        }
        return ab;
    }

    public T LoadAsset<T>(string abName, string path) where T : Object
    {
#if UNITY_EDITOR
        if (!LoadFromAssetBundle)
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif
        return LoadAssetBundle(abName).LoadAsset<T>(path);
    }

    public void UnloadAssetBundle(string name, bool unloadObj = false)
    {
        if (_assetBundles.TryGetValue(name, out var ab))
        {
            ab.Unload(unloadObj);
            _assetBundles.Remove(name);
        }
    }
}
