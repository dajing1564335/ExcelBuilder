using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using Table;

public class ExcelBuilder
{
    static readonly string MessageFolder = Application.dataPath + "/Scripts/Message/";
    public static string TableFolder = Application.dataPath + "/Scripts/Table/";
    public static string RefFolder = Application.dataPath + "/Resources/ExcelData/Ref/";
    public static string DataFolder = Application.dataPath + "/Resources/ExcelData/Data/";

    static readonly string MsgCsvFolder = Application.dataPath.Replace("Assets", "Data/Message/csv/");
    static readonly string TableCsvFolder = Application.dataPath.Replace("Assets", "Data/Table/csv/");

    const string AssetRefFolder = "Assets/Resources/ExcelData/Ref/";
    const string AssetDataFolder = "Assets/Resources/ExcelData/Data/";
    const string ExcelBuilderDataRefPath = AssetRefFolder + "ExcelBuilderData.asset";

    private static void CreateFolder()
    {
        Directory.CreateDirectory(TableFolder);
        Directory.CreateDirectory(RefFolder);
        Directory.CreateDirectory(DataFolder);
    }

    private static CsvDataTable LoadCsvToCsvDataTable(FileInfo info)
    {
        CsvDataTable tabel = new()
        {
            TableName = info.Name.Replace(".csv", ""),
            Rows = new()
        };
        if (File.Exists(info.FullName))
        {
            var csv = File.ReadAllText(info.FullName).Split("\r\n");
            for (int i = 0; i < csv.Length - 1; i++)
            {
                var line = csv[i].Split(',');
                if (line[0] != "comment")
                {
                    tabel.Rows.Add(line.ToList());
                }
            }
            if (tabel.Rows.Count > 0)
            {
                for (int i = 0; i < tabel.Rows[0].Count;)
                {
                    if (tabel.Rows[0][i] == "comment")
                    {
                        for (int j = 0; j < tabel.Rows.Count; j++)
                        {
                            tabel.Rows[j].RemoveAt(i);
                        }
                        continue;
                    }
                    i++;
                }
            }
        }
        return tabel;
    }

    #region BuildMsg
    [MenuItem("ExcelBuilder/BuildMsgAndLoad")]
    static void BuildMsg()
    {
        CreateFolder();
        string[] languages = default;
        List<string> msgLabels = new();
        var fileInfos = Directory.CreateDirectory(MsgCsvFolder).GetFiles("*.csv", SearchOption.AllDirectories);
        foreach (var file in fileInfos)
        {
            var table = LoadCsvToCsvDataTable(file);

            //GetLanguage,MsgLabel
            if (table.Rows.Count < 2 || table.Columns.Count < 2)
            {
                Debug.LogWarning($"Tabel [{file.Name}] is skipped.");
                continue;
            }
            if (languages == default)
            {
                languages = new string[table.Columns.Count - 1];
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    if (table.Rows[0][i] == string.Empty)
                    {
                        Debug.LogError($"Language can not be empty.[{file.Name}]");
                        return;
                    }
                    var language = table.Rows[0][i];
                    for (int j = 0; j < i - 1; j++)
                    {
                        if (languages[j] == language)
                        {
                            Debug.LogError($"There are same language.[{file.Name}]");
                            return;
                        }
                    }
                    languages[i - 1] = language;
                }
            }
            else
            {
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    if (languages[i - 1] != table.Rows[0][i])
                    {
                        Debug.LogError($"All message tabel's first row must be the same.[{file.Name}]");
                        return;
                    }
                }
            }
            for (int i = 1; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][0] == string.Empty)
                {
                    continue;
                }
                var msgLabel = table.Rows[i][0];
                if (msgLabels.Contains(msgLabel))
                {
                    Debug.LogError($"MsgLabel not unique.[{file.Name}-{table.TableName}-{msgLabel}]");
                    return;
                }
                msgLabels.Add(msgLabel);
            }
        }
        CreateLanguage(languages);
        CreateMsgLabel(msgLabels);
        var builderData = AssetDatabase.LoadAssetAtPath<ExcelBuilderSO>(ExcelBuilderDataRefPath);
        if (!builderData)
        {
            builderData = ScriptableObject.CreateInstance<ExcelBuilderSO>();
            AssetDatabase.CreateAsset(builderData, ExcelBuilderDataRefPath);
        }
        builderData.LoadMsgData = true;
        EditorUtility.SetDirty(builderData);
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
        Debug.Log("Build msg end. Wait refresh.");
    }

    private static void CreateLanguage(string[] languages)
    {
        var code = new StringBuilder();
        code.AppendLine("public enum Language");
        code.AppendLine("{");
        foreach (var language in languages)
        {
            code.AppendLine($"\t{language},");
        }
        code.AppendLine("}");

        File.WriteAllText(MessageFolder + "Language.cs", code.ToString());
    }

    private static void CreateMsgLabel(List<string> msgLabels)
    {
        var path = AssetRefFolder + "MsgRef.asset";
        var msgRef = AssetDatabase.LoadAssetAtPath<LabelRefSO>(path);
        if (!msgRef)
        {
            msgRef = ScriptableObject.CreateInstance<LabelRefSO>();
            AssetDatabase.CreateAsset(msgRef, path);
        }

        var code = new StringBuilder();
        code.AppendLine("public enum MsgLabel");
        code.AppendLine("{");
        foreach (var label in msgLabels)
        {
            code.AppendLine($"\t{label} = {msgRef.GetId(label)},");
        }
        code.AppendLine("}");

        EditorUtility.SetDirty(msgRef);
        AssetDatabase.SaveAssets();

        File.WriteAllText(MessageFolder + "MsgLabel.cs", code.ToString());
    }

    [MenuItem("ExcelBuilder/LoadMsgData")]
    private static void CreateMsgData()
    {
        var builderData = AssetDatabase.LoadAssetAtPath<ExcelBuilderSO>(ExcelBuilderDataRefPath);
        if (!builderData)
        {
            Debug.LogError("Please build first.");
            return;
        }
        if (builderData.LoadMsgData)
        {
            builderData.LoadMsgData = false;
            EditorUtility.SetDirty(builderData);
            AssetDatabase.SaveAssets();
        }
        var path = AssetDataFolder + "MsgData.asset";
        var msgData = AssetDatabase.LoadAssetAtPath<MessageSO>(path);
        if (!msgData)
        {
            msgData = ScriptableObject.CreateInstance<MessageSO>();
            AssetDatabase.CreateAsset(msgData, path);
        }
        msgData.Clear();
        var fileInfos = Directory.CreateDirectory(MsgCsvFolder).GetFiles("*.csv", SearchOption.AllDirectories);
        foreach (var file in fileInfos)
        {
            msgData.AddData(LoadCsvToCsvDataTable(file));
        }
        EditorUtility.SetDirty(msgData);
        AssetDatabase.SaveAssets();

        Debug.Log("Load msg data end.");
    }
    #endregion

    #region BuildTable
    class Field
    {
        public string Type;             //当前字段的类型
        public string Name;             //当前字段的那名字
        public int FieldLength = 1;     //当前字段类型所需的列数，--Vector3就是3
        public int ListCount = 1;       //是数组的话，数组的长度
        public int ListItemLength;      //是数组的话，数组成员字段类型所需的列数
        public bool IsBaseType;         //是基础类型吗
        public List<Field> SubClass;    //是在表中定义的子类的话，子类包含的所有字段

        public Field(string types, string name, bool isBaseType)
        {
            Type = types;
            Name = name;
            IsBaseType = isBaseType;
        }
    }

    struct ClassInfo
    {
        public string Name;
        public bool Dic;

        public ClassInfo(string name, bool dic)
        {
            Name = name;
            Dic = dic;
        }
    }

    [MenuItem("ExcelBuilder/BuildTableAndLoad")]
    private static void BuildTables()
    {
        CreateFolder();

        List<string> tableNames = new();
        List<CsvDataTable> csvData = new();

        #region Create Table Enum
        var fileInfos = Directory.CreateDirectory(TableCsvFolder).GetFiles("*.csv", SearchOption.AllDirectories);
        foreach (var file in fileInfos)
        {
            //LoadData
            var table = LoadCsvToCsvDataTable(file);
            if (table.Rows.Count < 3 || table.Columns.Count < 2)
            {
                Debug.LogWarning($"Tabel [{file.Name}] is skipped.");
                continue;
            }
            csvData.Add(table);
            List<string> labels = new();
            for (int i = 2; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][0] == string.Empty)
                {
                    continue;
                }
                var label = table.Rows[i][0];
                if (labels.Contains(label))
                {
                    Debug.LogError($"Label not unique.[{file.Name}-{label}]");
                    return;
                }
                labels.Add(label);
            }
            if (tableNames.Contains(table.TableName))
            {
                Debug.LogError($"Table not unique.[{file.Name}]");
                return;
            }
            else
            {
                CreateTableEnum(table.TableName, labels, table.Rows[0][0].ToString() == "ref");
                tableNames.Add(table.TableName);
            }
        }
        #endregion

        //获得表格第一行startIndex列之后空白格数
        int GetEmptyCount(int startIndex, CsvDataTable table)
        {
            int count = 0;
            for (int i = startIndex; i < table.Columns.Count; i++)
            {
                var type = table.Rows[0][i];
                if (type == "]")
                {
                    return count;
                }
                if (type == string.Empty)
                {
                    count++;
                }
                else
                {
                    Debug.LogError($"Miss \"]\" at [{table.TableName}] - (0,{i})");
                    return -1;
                }
            }
            Debug.LogError($"Miss \"]\" at [{table.TableName}]");
            return -1;
        }

        //获得表格从startIndex开始的类的第一个字段
        Field GetField(int startIndex, CsvDataTable table)
        {
            Field field;
            var type = table.Rows[0][startIndex];
            if (type == string.Empty)
            {
                Debug.LogError($"Table has empty type! [{table.TableName} - (0,{startIndex})]");
                return null;
            }
            var name = table.Rows[1][startIndex];
            if (name == string.Empty)
            {
                Debug.LogError($"Table has empty field! [{table.TableName} - (1,{startIndex})]");
                return null;
            }
            if (type == "class[")
            {
                field = new Field($"{table.TableName}{table.Rows[1][startIndex]}", table.Rows[1][startIndex], true);
                var fields = GetFields(startIndex + 1, table, true);
                if (fields == null)
                {
                    return null;
                }
                field.SubClass = fields;
                foreach (var f in fields)
                {
                    field.ListItemLength += f.FieldLength;
                }
                var count = GetEmptyCount(startIndex + 1 + field.ListItemLength, table);
                if (count == -1)
                {
                    return null;
                }
                if (count % field.ListItemLength != 0)
                {
                    Debug.LogError($"SubClass count is not correct! [{table.TableName} - {field.Name}");
                    return null;
                }
                field.ListCount += count / field.ListItemLength;
                field.FieldLength = field.ListCount * field.ListItemLength + 2;
            }
            else
            {
                var typeList = type.Split(";");
                var isBaseType = TypeConvert.SupportType.ContainsKey(typeList[0]);
                if (typeList.Length == 1 || type[^1] == ';')
                {
                    if (!isBaseType && !tableNames.Contains(typeList[0]) && Type.GetType($"{typeList[0]},Assembly-CSharp") == null)
                    {
                        Debug.LogError($"There are not support type! [{table.TableName} - (0,{startIndex}) - {typeList[0]}]");
                        return null;
                    }
                }
                else
                {
                    foreach (var t in typeList)
                    {
                        if (!tableNames.Contains(t) && Type.GetType($"{t},Assembly-CSharp") == null)
                        {
                            Debug.LogError($"Muilt type must be table enum! [{type}-{t}]");
                            return null;
                        }
                    }
                }
                field = new Field(type, name, isBaseType);
                if (startIndex + 1 < table.Columns.Count && table.Rows[0][startIndex + 1] == "[")
                {
                    field.ListItemLength = isBaseType ? TypeConvert.SupportType[typeList[0]] : 1;
                    var count = GetEmptyCount(startIndex + 2, table);
                    if (count == -1)
                    {
                        return null;
                    }
                    if ((count + 2) % field.ListItemLength != 0)
                    {
                        Debug.LogError($"{typeList[0]} count is not correct! [{table.TableName} - {field.Name}");
                        return null;
                    }
                    field.ListCount = (count + 2) / field.ListItemLength;
                    field.FieldLength = field.ListCount * field.ListItemLength + 1;
                }
                else
                {
                    field.FieldLength = isBaseType ? TypeConvert.SupportType[typeList[0]] : 1;
                }
            }
            return field;
        }

        //获得表格从startIndex开始的类的所有字段
        List<Field> GetFields(int startIndex, CsvDataTable table, bool sub = false)
        {
            List<Field> fields = new();
            while (startIndex < table.Columns.Count)
            {
                if (sub)
                {
                    var type = table.Rows[0][startIndex];
                    //子类的字段定义在第一个空格或者]处停止
                    if (type == string.Empty || type == "]")
                    {
                        return fields;
                    }
                }
                var field = GetField(startIndex, table);
                if (field == null)
                {
                    return null;
                }
                fields.Add(field);
                startIndex += field.FieldLength;
            }
            if (sub)
            {
                return null;
            }
            return fields;
        }

        //CreateDataClass
        List<ClassInfo> classInfos = new();

        foreach (var table in csvData)
        {
            List<Field> fields = GetFields(1, table);
            if (fields == null)
            {
                continue;
            }

            CreateTableDataClass(table.TableName, fields);
            var dic = table.Rows[0][0] == "ref";
            CreateTableSO(table.TableName, fields, dic);

            classInfos.Add(new ClassInfo(table.TableName, dic));
        }
        CreateTableAccessor(classInfos);

        var builderData = AssetDatabase.LoadAssetAtPath<ExcelBuilderSO>(ExcelBuilderDataRefPath);
        if (!builderData)
        {
            builderData = ScriptableObject.CreateInstance<ExcelBuilderSO>();
            AssetDatabase.CreateAsset(builderData, ExcelBuilderDataRefPath);
        }
        builderData.Updata(tableNames);
        builderData.LoadTableData = true;
        EditorUtility.SetDirty(builderData);
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
        Debug.Log("Build table end. Wait refresh.");
    }

    private static void CreateTableEnum(string name, List<string> labels, bool needRef)
    {
        var folder = TableFolder + name + "/";
        Directory.CreateDirectory(folder);

        LabelRefSO tabelRef = default;
        if (needRef)
        {
            var path = AssetRefFolder + name + "Ref.asset";
            tabelRef = AssetDatabase.LoadAssetAtPath<LabelRefSO>(path);
            if (!tabelRef)
            {
                tabelRef = ScriptableObject.CreateInstance<LabelRefSO>();
                AssetDatabase.CreateAsset(tabelRef, path);
            }
        }

        var code = new StringBuilder();
        code.AppendLine("namespace Table");
        code.AppendLine("{");
        code.AppendLine($"\tpublic enum {name}");
        code.AppendLine("\t{");
        if (needRef)
        {
            foreach (var field in labels)
            {
                code.AppendLine($"\t\t{field} = {tabelRef.GetId(field)},");
            }
            EditorUtility.SetDirty(tabelRef);
            AssetDatabase.SaveAssets();
        }
        else
        {
            foreach (var field in labels)
            {
                code.AppendLine($"\t\t{field},");
            }
        }
        code.AppendLine("\t}");
        code.AppendLine("}");

        File.WriteAllText(folder + name + ".cs", code.ToString());
    }

    private static void CreateTableDataClass(string name, List<Field> fields)
    {
        string GetType(Field field)
        {
            var type = field.Type.Contains(';') ? "int" : field.Type;
            return field.ListCount < 2 ? type : $"List<{type}>";
        }

        var code = new StringBuilder();
        void CreateClass(string name, List<Field> fields)
        {
            code.AppendLine("\t[System.Serializable]");
            code.AppendLine($"\tpublic class {name}");
            code.AppendLine("\t{");
            foreach (var field in fields)
            {
                code.AppendLine($"\t\tpublic {GetType(field)} {field.Name}{(field.ListCount < 2 ? ";" : " = new();")}");
            }
            code.AppendLine("\t}");
        }

        void CreateSubClass(Field field)
        {
            if (field.SubClass != null)
            {
                code.AppendLine();
                CreateClass(field.Type, field.SubClass);
                foreach (var f in field.SubClass)
                {
                    CreateSubClass(f);
                }
            }
        }

        var folder = TableFolder + name + "/";
        Directory.CreateDirectory(folder);

        code.AppendLine("using System.Collections.Generic;");
        code.AppendLine("using UnityEngine;");
        code.AppendLine();
        code.AppendLine("namespace Table");
        code.AppendLine("{");
        CreateClass(name + "Data", fields);
        foreach (var field in fields)
        {
            CreateSubClass(field);
        }
        code.AppendLine("}");

        File.WriteAllText(folder + name + "Data.cs", code.ToString());
    }

    private static void CreateTableSO(string name, List<Field> fields, bool needRef)
    {
        string GetSpace(int count)
        {
            string space = "";
            for (int i = 0; i < count; i++)
            {
                space += "\t";
            }
            return space;
        }

        string GetFieldValue(Field field, int index, int j, int loop = 0)
        {
            if (field.IsBaseType)
            {
                StringBuilder code = new();
                if (field.SubClass == null)
                {
                    code.Append($"TypeConvert.GetValue<{field.Type}>(row[{(loop < 0 ? index : j == 0 ? $"j{loop}" : $"j{loop} + {j}")}]");
                    for (int i = 1; i < TypeConvert.SupportType[field.Type]; i++)
                    {
                        code.Append($", row[{(loop < 0 ? index + i : $"j{loop} + {i + j}")}]");
                    }
                    code.Append(")");
                    return code.ToString();
                }
                code.AppendLine($"{GetSpace(5 + loop)}{field.Type} {field.Name} = new();");
                foreach (var f in field.SubClass)
                {
                    code.AppendLine(GetFieldCode(index + j + 1, f, field.Name, loop < 0 ? j : f.ListCount > 1 || f.SubClass != null ? j + 1 : j, loop));
                    j += f.FieldLength;
                }
                return code.ToString();
            }
            if (field.Type.Contains(';'))
            {
                return $"TypeConvert.GetValue(row[{(loop < 0 ? index : j == 0 ? $"j{loop}" : $"j{loop} + {j}")}], \"{field.Type}\")";
            }
            return $"TypeConvert.GetValue<{field.Type}>(row[{(loop < 0 ? index : j == 0 ? $"j{loop}" : $"j{loop} + {j}")}])";
        }

        string GetFieldCode(int index, Field field, string dataName, int j, int loop)
        {
            var space = GetSpace(5 + loop);
            if (field.ListCount < 2)
            {
                if (!field.IsBaseType || field.SubClass == null)
                {
                    return $"{space}{dataName}.{field.Name} = {GetFieldValue(field, index, j, loop)};";
                }
                return $"{GetFieldValue(field, index, j, loop)}{space}{dataName}.{field.Name} = {field.Name};";
            }
            var code = new StringBuilder();
            index++;
            loop++;
            code.AppendLine($"{space}for (int j{loop} = {(loop == 0 ? index : $"j{loop - 1} + {j}")}; j{loop} < {(loop == 0 ? index + field.ListCount * field.ListItemLength : $"j{loop - 1} + {j + field.ListCount * field.ListItemLength}")}; j{loop} += {field.ListItemLength})");
            code.AppendLine($"{space}{{");
            code.AppendLine($"{space}\tif (row[j{loop}] == string.Empty)");
            code.AppendLine($"{space}\t{{");
            code.AppendLine($"{space}\t\tbreak;");
            code.AppendLine($"{space}\t}}");
            if (!field.IsBaseType || field.SubClass == null)
            { 
                code.AppendLine($"{space}\t{dataName}.{field.Name}.Add({GetFieldValue(field, index, 0, loop)});"); 
            }
            else
            {
                code.AppendLine($"{GetFieldValue(field, index, 0, loop)}{space}\t{dataName}.{field.Name}.Add({field.Name});");
            }
            code.Append($"{space}}}");
            return code.ToString();
        }

        var code = new StringBuilder();
        code.AppendLine("using System.Collections.Generic;");
        code.AppendLine("using UnityEngine;");
        code.AppendLine();
        code.AppendLine("namespace Table");
        code.AppendLine("{");
        code.AppendLine($"\tpublic class {name}SO : ScriptableObjectBase");
        code.AppendLine("\t{");
        var type = (needRef ? $"SerializableDictionary<{name}, " : "List<") + $"{name}Data>";
        code.AppendLine($"\t\tpublic {type} Datas;");
        code.AppendLine();
        code.AppendLine("#if UNITY_EDITOR");
        code.AppendLine("\t\tpublic override void CreateData(CsvDataTable table)");
        code.AppendLine("\t\t{");
        code.AppendLine($"\t\t\tDatas = new {type}();");
        code.AppendLine("\t\t\tfor (int i = 2; i < table.Rows.Count; i++)");
        code.AppendLine("\t\t\t{");
        code.AppendLine("\t\t\t\tvar row = table.Rows[i];");
        code.AppendLine("\t\t\t\tif (row[0] == string.Empty)");
        code.AppendLine("\t\t\t\t{");
        code.AppendLine("\t\t\t\t\tcontinue;");
        code.AppendLine("\t\t\t\t}");

        int index = 1;
        code.AppendLine($"\t\t\t\t{name}Data data = new();");
        foreach (var field in fields)
        {
            code.AppendLine(GetFieldCode(index, field, "data", 0, -1));
            index += field.FieldLength;
        }
        code.Append("\t\t\t\tDatas.Add(");
        if (needRef)
        {
            code.Append($"({name})System.Enum.Parse(typeof({name}), row[0].ToString()), ");
        }
        code.AppendLine($"data);");
        code.AppendLine("\t\t\t}");
        code.AppendLine("\t\t}");
        code.AppendLine("#endif");
        code.AppendLine("\t}");
        code.AppendLine("}");
        File.WriteAllText(TableFolder + name + "/" + name + "SO.cs", code.ToString());
    }

    private static void CreateTableAccessor(List<ClassInfo> infos)
    {
        var code = new StringBuilder();
        code.AppendLine("public static class TableAccessor");
        code.AppendLine("{");
        foreach (var info in infos)
        {
            code.AppendLine($"\tpublic static Table.TableAccessor{(info.Dic ? "Dictionary" : "List")}<Table.{info.Name}, Table.{info.Name}Data> {info.Name};");
        }
        code.AppendLine();
        code.AppendLine("\tpublic static void LoadData()");
        code.AppendLine("\t{");
        foreach (var info in infos)
        {
            code.AppendLine($"\t\t{info.Name} = new Table.TableAccessor{(info.Dic ? "Dictionary" : "List")}<Table.{info.Name}, Table.{info.Name}Data>();");
        }
        code.AppendLine("\t}");
        code.AppendLine("}");

        File.WriteAllText(TableFolder + "TableAccessor.cs", code.ToString());
    }


    [MenuItem("ExcelBuilder/LoadTableData")]
    static void CreateTableData()
    {
        var builderData = AssetDatabase.LoadAssetAtPath<ExcelBuilderSO>(ExcelBuilderDataRefPath);
        if (!builderData)
        {
            Debug.LogError("Please build first.");
            return;
        }
        if (builderData.LoadTableData)
        {
            builderData.LoadTableData = false;
            EditorUtility.SetDirty(builderData);
            AssetDatabase.SaveAssets();
        }

        foreach (var file in Directory.CreateDirectory(TableCsvFolder).GetFiles("*.csv", SearchOption.AllDirectories))
        {
            //LoadData
            var table = LoadCsvToCsvDataTable(file);
            if (table.Rows.Count < 3 || table.Columns.Count < 2)
            {
                continue;
            }
            //CreateTableData
            var path = AssetDataFolder + table.TableName + "Data.asset";
            var tableData = AssetDatabase.LoadAssetAtPath<Table.ScriptableObjectBase>(path);
            if (!tableData)
            {
                tableData = (Table.ScriptableObjectBase)ScriptableObject.CreateInstance(Type.GetType($"Table.{table.TableName}SO,Assembly-CSharp"));
                AssetDatabase.CreateAsset(tableData, path);
            }
            tableData.CreateData(table);
            EditorUtility.SetDirty(tableData);
            AssetDatabase.SaveAssets();
        }
        Debug.Log("Load table data end.");
    }
    #endregion

    [InitializeOnLoadMethod]
    public static void OnLoadMethod()
    {
        var builderData = AssetDatabase.LoadAssetAtPath<ExcelBuilderSO>(ExcelBuilderDataRefPath);
        if (!builderData || !(builderData.LoadMsgData || builderData.LoadTableData))
        {
            return;
        }
        if (builderData.LoadMsgData)
        {
            builderData.LoadMsgData = false;
            CreateMsgData();
        }
        if (builderData.LoadTableData)
        {
            builderData.LoadMsgData = false;
            CreateTableData();
        }
        EditorUtility.SetDirty(builderData);
        AssetDatabase.SaveAssets();
    }
}