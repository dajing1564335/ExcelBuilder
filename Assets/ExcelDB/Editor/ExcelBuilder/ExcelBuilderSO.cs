using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class ExcelBuilderSO : ScriptableObject
{
    public bool LoadMsgData;
    public bool LoadTableData;
    public List<string> EnumNames = new();
    public List<string> WarningLogs = new();

    public void Updata(List<string> names)
    {
        foreach (var name in EnumNames.Where(name => !names.Contains(name)))
        {
            if (Directory.Exists(ExcelBuilder.TableFolder + name))
            {
                Directory.Delete(ExcelBuilder.TableFolder + name, true);
            }
            File.Delete(ExcelBuilder.TableFolder + name + ".meta");
            File.Delete(ExcelBuilder.DataFolder + name + "Data.asset");
            File.Delete(ExcelBuilder.DataFolder + name + "Data.asset.meta");
            File.Delete(ExcelBuilder.RefFolder + name + "Ref.asset");
            File.Delete(ExcelBuilder.RefFolder + name + "Ref.asset.meta");
        }

        EnumNames = names;
    }
}