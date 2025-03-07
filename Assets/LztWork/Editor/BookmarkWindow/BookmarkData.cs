using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BookmarkData : ScriptableObject
{
    private const string ASSET_PATH = "Assets/LztWork/Editor/BookmarkWindow/BookmarkData.asset";

    public List<Object> Bookmarks = new();

    public static BookmarkData LoadOrCreate()
    {
        var data = AssetDatabase.LoadAssetAtPath<BookmarkData>(ASSET_PATH);
        if (!data)
        {
            data = CreateInstance<BookmarkData>();
            AssetDatabase.CreateAsset(data, ASSET_PATH);
            AssetDatabase.SaveAssets();
        }
        return data;
    }
}