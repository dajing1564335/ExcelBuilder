using System.Collections.Generic;
using UnityEngine;

public class LoadManager : SingletonMonoBehaviour<LoadManager>
{
    [System.Serializable]
    public class AssetBundleInfo
    {
        [SerializeField]
        AssetBundle _assetBundle;
        [SerializeField]
        int _count;

        public AssetBundle AssetBundle => _assetBundle;

        public AssetBundleInfo(AssetBundle assetBundle, int count)
        {
            _assetBundle = assetBundle;
            _count = count;
        }

        public void Add()
        {
            _count++;
        }

        public bool Dec()
        {
            return --_count <= 0;
        }
    }

    public static readonly string StreamFolderWindows = Application.streamingAssetsPath + "/Windows/";

#if UNITY_EDITOR
    [SerializeField]
    private bool LoadFromAssetBundle;
    [SerializeField]
    private SerializableDictionary<string, AssetBundleInfo> _assetBundles = new();
#else
    private readonly Dictionary<string, AssetBundleInfo> _assetBundles = new();
#endif

    AssetBundleManifest _manifest;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        if (!LoadFromAssetBundle)
        {
            return;
        }
#endif
        var ab = AssetBundle.LoadFromFile(StreamFolderWindows + "Windows");
        _manifest = ab.LoadAsset<AssetBundleManifest>("AssetbundleManifest");
        ab.Unload(false);
    }

    private AssetBundle LoadAssetBundle(string name, bool dependency)
    {
        if (!_assetBundles.TryGetValue(name, out var info))
        {
            var asset = AssetBundle.LoadFromFile(StreamFolderWindows + name);
            if (asset)
            {
                info = new AssetBundleInfo(asset, dependency ? 1 : 0);
                _assetBundles.Add(name, info);
                foreach (var sub in _manifest.GetAllDependencies(name))
                {
                    LoadAssetBundle(sub, true);
                }
            }
            else
            {
                Debug.LogError($"Load AssetBundle Error. [{name}]");
            }
        }
        else if (dependency)
        {
            info.Add();
        }
        return info.AssetBundle;
    }

    public T LoadAsset<T>(string abName, string path) where T : Object
    {
#if UNITY_EDITOR
        if (!LoadFromAssetBundle)
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif
        return LoadAssetBundle(abName, false).LoadAsset<T>(path);
    }

    public void UnloadAssetBundle(string name, bool dependency = false, bool unload = false)
    {
        if (_assetBundles.TryGetValue(name, out var info))
        {
            if (!dependency || info.Dec())
            {
                info.AssetBundle.Unload(unload);
                _assetBundles.Remove(name);
            }
            foreach (var sub in _manifest.GetAllDependencies(name))
            {
                UnloadAssetBundle(sub, true, unload);
            }
        }
    }
}
