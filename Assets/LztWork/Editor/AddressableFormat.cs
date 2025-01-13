using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class AddressableFormat
{
    [MenuItem("Addressable/Create")]
    private static void Create()
    {
        var type = typeof(AssetEnum);
        var names = Enum.GetNames(type).ToList();
        var values = (int[])Enum.GetValues(type);
        var max = values.Max();
        
        var code = new StringBuilder();
        code.Append("public enum AssetEnum\n");
        code.Append("{\n");

        List<char> ngChar = new() { ' ', '-' };

        List<string> ignoreGroup = new() { "Built In Data", "DebugAssets" };
        //List<string> groups = new();
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            var name = group.Name;
            if (ignoreGroup.Contains(name))
            {
                continue;
            }
            //groups.Add(name);
            foreach (var entry in group.entries)
            {
                var address = Path.GetFileNameWithoutExtension(entry.address);
                foreach (var ch in ngChar)
                {
                    address = address.Replace(ch, '_');
                }
                if (address != entry.address)
                {
                    entry.SetAddress(address);
                }
                var index = names.IndexOf(address);
                code.Append($"\t{entry.address} = {(index >= 0 ? values[index] : ++max)},\n");
            }
        }

        code.Append("}\n");
        File.WriteAllText("Assets/Scripts/Common/AssetEnum.cs", code.ToString());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
