using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ExcelBuilderSO : ScriptableObject
{
    public bool NeedRebuild;
    public List<string> EnumNames = new List<string>();

    public void Updata(List<string> names)
    {
        foreach (var name in EnumNames)
        {
            if (!names.Contains(name))
            {
                Directory.Delete(ExcelBuilder.TableFolder + name, true);
                File.Delete(ExcelBuilder.TableFolder + name + ".meta");
                File.Delete(ExcelBuilder.DataFolder + name + "Data.asset");
                File.Delete(ExcelBuilder.DataFolder + name + "Data.asset.meta");
                File.Delete(ExcelBuilder.RefFolder + name + "Ref.asset");
                File.Delete(ExcelBuilder.RefFolder + name + "Ref.asset.meta");
            }
        }
        EnumNames = names;
    }
}
