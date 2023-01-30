using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder : MonoBehaviour
{
    static readonly string StreamFolderWindows = Application.streamingAssetsPath + "/Windows/";

    [MenuItem("AssetBunlde/BuildWindows")]
    static void BuildWindows()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath);
        Directory.CreateDirectory(StreamFolderWindows);
        BuildPipeline.BuildAssetBundles(StreamFolderWindows, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
    }
}
