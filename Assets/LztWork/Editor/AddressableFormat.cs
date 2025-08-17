using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;

public class AddressableFormat
{
    private const string AssetEnumPath = "Assets/LztWork/Runtime/Manager/AssetManager/AssetEnum.cs";
    
    [MenuItem("Addressable/Create")]
    private static void Create()
    {
        var type = typeof(AssetEnum);
        var names = Enum.GetNames(type).ToList();
        var values = (int[])Enum.GetValues(type);
        var max = values.Length > 0 ? values.Max() : -1;
        
        var code = new StringBuilder();
        code.Append("public enum AssetEnum\n");
        code.Append("{\n");

        List<char> ngChar = new() { ' ', '-' };

        List<string> ignoreGroup = new() { "Built In Data", "DebugAssets" };
        //List<string> groups = new();
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            var name = group.Name;
            if (ignoreGroup.Contains(name))
            {
                continue;
            }
            //groups.Add(name);
            if (!settings.GetLabels().Contains(name))
            {
                settings.AddLabel(name);
            }

            foreach (var entry in group.entries)
            {
                var address = Path.GetFileNameWithoutExtension(entry.address);
                address = ngChar.Aggregate(address, (current, ch) => current.Replace(ch, '_'));
                if (address != entry.address)
                {
                    entry.SetAddress(address);
                }
                if (!entry.labels.Contains(name))
                {
                    entry.labels.Clear();
                    entry.labels.Add(name);
                }
                var index = names.IndexOf(address);
                code.Append($"\t{entry.address} = {(index >= 0 ? values[index] : ++max)},\n");
            }
        }

        code.Append("}\n");
        File.WriteAllText(AssetEnumPath, code.ToString());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
