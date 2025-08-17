using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetManager : SingletonMonoBehaviour<AssetManager>
{
    private class AssetHandle
    {
        public AsyncOperationHandle Handle;
        public int Count = 1;

        public AssetHandle(AsyncOperationHandle handle)
        {
            Handle = handle;
        }
    }
    
    private readonly Dictionary<AssetEnum, AssetHandle> _cache = new();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadAsset<T>(AssetEnum asset, System.Action<T> action = null)
    {
        if (_cache.TryGetValue(asset, out var h))
        {
            h.Count++;
            if (action == null) return;
            if (h.Handle.Status == AsyncOperationStatus.Succeeded)
            {
                action.Invoke((T)h.Handle.Result);
            }
            else
            {
                h.Handle.Completed += op =>
                {
                    if (_cache[asset].Count > 0)
                    {
                        action.Invoke((T)op.Result);
                    }
                };
            }
        }
        else
        {
            var handle = Addressables.LoadAssetAsync<T>(asset.ToString());
            _cache.Add(asset, new AssetHandle(handle));
            handle.Completed += op =>
            {
                if (_cache[asset].Count <= 0)
                {
                    Addressables.Release(op);
                    _cache.Remove(asset);
                }
                else
                {
                    action?.Invoke(op.Result);
                }
            };
        }
    }

    public bool AllLoaded() => _cache.Values.All(item => item.Handle.Status == AsyncOperationStatus.Succeeded);

    public bool IsLoaded(AssetEnum asset) => _cache.TryGetValue(asset, out var handle) && handle.Handle.Status == AsyncOperationStatus.Succeeded;

    public T GetAsset<T>(AssetEnum asset) => IsLoaded(asset) && _cache[asset].Handle.Result is T ret ? ret : default;

    public void UnloadAsset(AssetEnum asset)
    {
        if (!_cache.TryGetValue(asset, out var handle)) return;
        if (--handle.Count > 0) return;
        if (handle.Handle.Status != AsyncOperationStatus.Succeeded) return;
        Addressables.Release(handle.Handle);
        _cache.Remove(asset);
    }

    public void LoadAssetsWithLabel<T>(string label)
    {
        Addressables.LoadResourceLocationsAsync(label).Completed += locations =>
        {
            foreach (var location in locations.Result)
            {
                if (Enum.TryParse(location.PrimaryKey, out AssetEnum assetEnum))
                {
                    LoadAsset<T>(assetEnum);
                }
            }
        };
    }

    public void UnloadAssetsWithLabel(string label)
    {
        Addressables.LoadResourceLocationsAsync(label).Completed += locations =>
        {
            foreach (var location in locations.Result)
            {
                if (Enum.TryParse(location.PrimaryKey, out AssetEnum assetEnum))
                {
                    UnloadAsset(assetEnum);
                }
            }
        };
    }
}
