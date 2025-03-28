using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Table
{
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
                ExcelBuilder.DeleteTableFile(name);
            }

            EnumNames = names;
        }
    }
}