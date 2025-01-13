using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build;

public class Builder
{
    private const string ExeDebugPath = "Rom/Debug/Card.exe";
    private const string ExeReleasePath = "Rom/Release/Card.exe";
    
    private const string DEBUG_BUILD = "DEBUG_BUILD";
    private const string DEBUG_ASSETS = "DebugAssets";

    [MenuItem("Build/Build Debug")]
    public static void BuildDebug()
    {
        BundledAssetGroupSchema schema = default;

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        foreach (var group in settings.groups)
        {
            if (group.Name == DEBUG_ASSETS)
            {
                schema = group.GetSchema<BundledAssetGroupSchema>();
                schema.IncludeInBuild = true;
                break;
            }
        }


        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, DEBUG_BUILD);
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, ExeDebugPath, BuildTarget.StandaloneWindows, BuildOptions.Development | BuildOptions.AllowDebugging);
    
        schema.IncludeInBuild = false;
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Build/Build Release")]
    public static void BuildRelease()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        foreach (var group in settings.groups)
        {
            if (group.Name == DEBUG_ASSETS)
            {
                group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = false;
                break;
            }
        }


        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, "");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, ExeReleasePath, BuildTarget.StandaloneWindows, BuildOptions.None);
    }
}
