using System.IO;
using UnityEditor;
using UnityEngine;
using ExcelDataReader;
using System.Data;
using System.Collections.Generic;
using System;
using System.Text;

public class ExcelBuilder
{
    static readonly string MessageFolder = Application.dataPath + "/Scripts/Message/";
    public static string TableFolder = Application.dataPath + "/Scripts/Table/";
    public static string RefFolder = Application.dataPath + "/Resources/Ref/";
    public static string DataFolder = Application.dataPath + "/Resources/ExcelData/";

    static readonly string MsgExcelPath = Application.dataPath.Replace("Assets", "Data/Message/Message.xlsx");
    static readonly string TableExcelFolder = Application.dataPath.Replace("Assets", "Data/Table/");

    private static void CreateFolder()
    {
        Directory.CreateDirectory(TableFolder);
        Directory.CreateDirectory(RefFolder);
        Directory.CreateDirectory(DataFolder);
    }

    #region BuildMsg
    [MenuItem("ExcelBuilder/BuildMsgAndLoad")]
    static void BuildMsg()
    {
        CreateFolder();
        //LoadData
        var steam = File.OpenRead(MsgExcelPath);
        var reader = ExcelReaderFactory.CreateOpenXmlReader(steam);
        var tables = reader.AsDataSet().Tables;
        reader.Close();
        steam.Close();

        //GetLanguage,MsgLabel
        string[] languages = default;
        List<string> msgLabels = new List<string>();
        foreach (DataTable table in tables)
        {
            if (table.Rows.Count == 0 || table.Columns.Count < 2)
            {
                Debug.LogWarning($"Tabel [{table.TableName}] is skipped.");
                continue;
            }
            if (languages == default)
            {
                languages = new string[table.Columns.Count - 1];
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    if (table.Rows[0][i] is DBNull)
                    {
                        Debug.LogError($"Language can not be empty.[{table.TableName}]");
                        return;
                    }
                    var language = table.Rows[0][i].ToString();
                    for (int j = 0; j < i - 1; j++)
                    {
                        if (languages[j] == language)
                        {
                            Debug.LogError($"There are same language.[{table.TableName}]");
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
                    if (languages[i - 1] != table.Rows[0][i].ToString())
                    {
                        Debug.LogError($"All message tabel's first row must be the same.[{table.TableName}]");
                        return;
                    }
                }
            }
            for (int i = 1; i < table.Rows.Count; i++)
            {
                if(table.Rows[i][0] is DBNull)
                {
                    continue;
                }
                var msgLabel = table.Rows[i][0].ToString();
                if (msgLabels.Contains(msgLabel))
                {
                    Debug.LogError($"MsgLabel not unique.[{msgLabel}]");
                    return;
                }
                msgLabels.Add(msgLabel);
            }
        }
        CreateLanguage(languages);
        CreateMsgLabel(msgLabels);

        var builderData = Resources.Load<ExcelBuilderSO>("Ref/ExcelBuilderData");
        if (!builderData)
        {
            builderData = ScriptableObject.CreateInstance<ExcelBuilderSO>();
            AssetDatabase.CreateAsset(builderData, "Assets/Resources/Ref/ExcelBuilderData.asset");
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
        var msgRef = Resources.Load<LabelRefSO>("Ref/MsgRef");
        if (!msgRef)
        {
            msgRef = ScriptableObject.CreateInstance<LabelRefSO>();
            AssetDatabase.CreateAsset(msgRef, "Assets/Resources/Ref/MsgRef.asset");
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
        var builderData = Resources.Load<ExcelBuilderSO>($"Ref/ExcelBuilderData");
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

        var steam = File.OpenRead(MsgExcelPath);
        var reader = ExcelReaderFactory.CreateOpenXmlReader(steam);
        var tables = reader.AsDataSet().Tables;
        reader.Close();
        steam.Close();

        var msgData = Resources.Load<MessageSO>("ExcelData/MsgData");
        if (!msgData)
        {
            msgData = ScriptableObject.CreateInstance<MessageSO>();
            AssetDatabase.CreateAsset(msgData, "Assets/Resources/ExcelData/MsgData.asset");
        }
        msgData.CreateData(tables);
        EditorUtility.SetDirty(msgData);
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
        Debug.Log("Load msg data end.");
    }
    #endregion

    #region BuildTable
    class Field
    {
        public string Type;
        public string Name;
        public int FieldLength = 1;
        public int ListCount = 1;
        public int ListItemLength;
        public bool IsBaseType;
        public List<Field> SubClass;

        public Field(string types, string name, bool isBaseType)
        {
            Type = types;
            Name = name;
            IsBaseType = isBaseType;
        }
    }

    static string GetTableName(string name, string table0Name, string fileName)
    {
        return (name == "Sheet1" && name == table0Name) ? fileName.Replace(".xlsx", string.Empty) : name;
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

        List<string> folderNames = new();
        List<DataTableCollection> excelData = new();

        var fileInfos = Directory.CreateDirectory(TableExcelFolder).GetFiles("*.xlsx", SearchOption.AllDirectories);
        foreach (var file in fileInfos)
        {
            //LoadData
            var steam = File.OpenRead(file.FullName);
            var reader = ExcelReaderFactory.CreateOpenXmlReader(steam);
            var tables = reader.AsDataSet().Tables;
            excelData.Add(tables);
            reader.Close();
            steam.Close();
            
            foreach (DataTable table in tables)
            {
                if (table.Columns.Count == 0 || table.Rows.Count < 2)
                {
                    continue;
                }
                List<string> labels = new();
                for (int i = 2; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][0] is DBNull || table.Rows[i][0].ToString() == "comment")
                    {
                        continue;
                    }
                    var label = table.Rows[i][0].ToString();
                    if (labels.Contains(label))
                    {
                        Debug.LogError($"Label not unique.[{file.Name}]-[{label}]");
                        return;
                    }
                    labels.Add(label);
                }
                var folderName = GetTableName(table.TableName, tables[0].TableName, file.Name);
                CreateTableEnum(folderName, labels, table.Rows[0][0].ToString() == "ref");

                folderNames.Add(folderName);
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        
        int GetEmptyCount(int startIndex, DataTable table, int index)
        {
            int count = 0;
            for (int i = startIndex; i < table.Columns.Count; i++)
            {
                var type = table.Rows[0][i].ToString();
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
                    Debug.LogError($"Miss \"]\" at [{fileInfos[index].Name} - {table.TableName}]");
                    return -1;
                }
            }
            Debug.LogError($"Miss \"]\" at [{fileInfos[index].Name} - {table.TableName}]");
            return -1;
        }

        Field GetField(int startIndex, DataTable table, int index)
        {
            Field field;
            var type = table.Rows[0][startIndex].ToString();
            if (type == string.Empty)
            {
                Debug.LogError($"Table has empty type! [{fileInfos[index].Name} - {table.TableName} - (0,{startIndex})]");
                return null;
            }
            var name = table.Rows[1][startIndex].ToString();
            if (name == string.Empty)
            {
                Debug.LogError($"Table has empty field! [{fileInfos[index].Name} - {table.TableName} - (1,{startIndex})]");
                return null;
            }
            if (type == "class[")
            {
                field = new Field($"{GetTableName(table.TableName, excelData[index][0].TableName, fileInfos[index].Name)}{table.Rows[1][startIndex]}", table.Rows[1][startIndex].ToString(), true);
                var fields = GetFields(startIndex + 1, table, index, true);
                if (fields == null)
                {
                    return null;
                }
                field.SubClass = fields;
                foreach (var f in fields)
                {
                    field.ListItemLength += f.FieldLength;
                }
                var count = GetEmptyCount(startIndex + 1 + field.ListItemLength, table, index);
                if (count == -1)
                {
                    return null;
                }
                if (count % field.ListItemLength != 0)
                {
                    Debug.LogError($"SubClass count is not correct! [{fileInfos[index].Name} - {table.TableName} - {field.Name}");
                    return null;
                }
                field.ListCount += count / field.ListItemLength;
                field.FieldLength = field.ListCount * field.ListItemLength + 2;
            }
            else
            {
                var typeList = type.Split(";");
                var isBaseType = TypeConvert.SupportType.Contains(typeList[0]);
                if (typeList.Length == 1)
                {
                    if (!isBaseType && !folderNames.Contains(typeList[0]) && Type.GetType($"{typeList[0]},Assembly-CSharp") == null)
                    {
                        Debug.LogError($"There are not support type! [{fileInfos[index].Name} - {table.TableName} - (0,{startIndex}) - {typeList[0]}]");
                        return null;
                    }
                }
                else
                {
                    foreach (var t in typeList)
                    {
                        if (!folderNames.Contains(t) && Type.GetType($"{t},Assembly-CSharp") == null)
                        {
                            Debug.LogError($"Muilt type must be table enum! [{type}-{t}]");
                            return null;
                        }
                    }
                }
                field = new Field(type, name, isBaseType);
                if (startIndex + 1 < table.Columns.Count && table.Rows[0][startIndex + 1].ToString() == "[")
                {
                    field.ListItemLength = 1;
                    var count = GetEmptyCount(startIndex + 2, table, index);
                    if (count == -1)
                    {
                        return null;
                    }
                    field.ListCount += count + 1;
                    field.FieldLength = field.ListCount + 1;
                }
            }
            return field;
        }

        List<Field> GetFields(int startIndex, DataTable table, int index, bool sub = false)
        {
            List<Field> fields = new();
            while (startIndex < table.Columns.Count)
            {
                if (sub)
                {
                    var type = table.Rows[0][startIndex].ToString();
                    if (type == string.Empty || type == "]")
                    {
                        return fields;
                    }
                }
                var field = GetField(startIndex, table, index);
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

        for (int index = 0; index < fileInfos.Length; ++index)
        {
            foreach (DataTable table in excelData[index])
            {
                if (table.Columns.Count < 2 || table.Rows.Count < 2)
                {
                    continue;
                }
                List<Field> fields = GetFields(1, table, index);
                if (fields == null)
                {
                    return;
                }
                var tableName = GetTableName(table.TableName, excelData[index][0].TableName, fileInfos[index].Name);
                if (!folderNames.Contains(tableName))
                {
                    folderNames.Add(tableName);
                }

                CreateTableDataClass(tableName, fields);
                var dic = table.Rows[0][0].ToString() == "ref";
                CreateTableSO(tableName, fields, dic);

                classInfos.Add(new ClassInfo(tableName, dic));
            }
        }
        CreateTableAccessor(classInfos);

        var builderData = Resources.Load<ExcelBuilderSO>("Ref/ExcelBuilderData");
        if (!builderData)
        {
            builderData = ScriptableObject.CreateInstance<ExcelBuilderSO>();
            AssetDatabase.CreateAsset(builderData, "Assets/Resources/Ref/ExcelBuilderData.asset");
        }
        builderData.Updata(folderNames);
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
            var fileName = name + "Ref";
            tabelRef = Resources.Load<LabelRefSO>($"Ref/{fileName}");
            if (!tabelRef)
            {
                tabelRef = ScriptableObject.CreateInstance<LabelRefSO>();
                AssetDatabase.CreateAsset(tabelRef, $"Assets/Resources/Ref/{fileName}.asset");
            }
        }

        var code = new StringBuilder();
        code.AppendLine("namespace Table");
        code.AppendLine("{");
        code.AppendLine($"\tpublic enum {name}Enum");
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

        File.WriteAllText(folder + name + "Enum.cs", code.ToString());
    }

    private static void CreateTableDataClass(string name, List<Field> fields)
    {
        string GetType(Field field)
        {
            var type = field.IsBaseType ? field.Type : "int";
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
                if (field.SubClass == null)
                {
                    return $"TypeConvert.GetValue<{field.Type}>(table.Rows[i][{(loop < 0 ? index : j == 0 ? $"j{loop}" : $"j{loop} + {j}")}].ToString())";
                }
                var code = new StringBuilder();
                code.AppendLine($"{GetSpace(4 + loop)}Table.{field.Type} {field.Name} = new();");
                foreach (var f in field.SubClass)
                {
                    code.AppendLine(GetFieldCode(index + j + 1, f, field.Name, loop < 0 ? j : f.ListCount > 1 || f.SubClass != null ? j + 1 : j, loop));
                    j += f.FieldLength;
                }
                return code.ToString();
            }
            return $"TypeConvert.GetValue(table.Rows[i][{(loop < 0 ? index : j == 0 ? $"j{loop}" : $"j{loop} + {j}")}].ToString(), \"{field.Type}\")";
        }

        string GetFieldCode(int index, Field field, string dataName, int j, int loop)
        {
            var space = GetSpace(4 + loop);
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
            code.AppendLine($"{space}\tif (table.Rows[i][j{loop}] is System.DBNull)");
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
        code.AppendLine("using System.Data;");
        code.AppendLine();
        code.AppendLine($"public class {name}SO : ScriptableObjectBase");
        code.AppendLine("{");
        var type = (needRef ? $"SerializableDictionary<Table.{name}Enum, " : "List<") + $"Table.{name}Data>";
        code.AppendLine($"\tpublic {type} Datas;");
        code.AppendLine();
        code.AppendLine("\tpublic override void CreateData(DataTable table)");
        code.AppendLine("\t{");
        code.AppendLine($"\t\tDatas = new {type}();");
        code.AppendLine("\t\tfor (int i = 2; i < table.Rows.Count; i++)");
        code.AppendLine("\t\t{");
        code.AppendLine("\t\t\tif (table.Rows[i][0] is System.DBNull || table.Rows[i][0].ToString() == \"comment\")");
        code.AppendLine("\t\t\t{");
        code.AppendLine("\t\t\t\tcontinue;");
        code.AppendLine("\t\t\t}");

        int index = 1;
        code.AppendLine($"\t\t\tTable.{name}Data data = new();");
        foreach (var field in fields)
        {
            code.AppendLine(GetFieldCode(index, field, "data", 0, -1));
            index += field.FieldLength;
        }
        code.Append("\t\t\tDatas.Add(");
        if (needRef)
        {
            code.Append($"(Table.{name}Enum)System.Enum.Parse(typeof(Table.{name}Enum), table.Rows[i][0].ToString()), ");
        }
        code.AppendLine($"data);");
        code.AppendLine("\t\t}");
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
            code.AppendLine($"\tpublic static TableAccessor{(info.Dic ? "Dictionary" : "List")}<Table.{info.Name}Enum, Table.{info.Name}Data> {info.Name};");
        }
        code.AppendLine();
        code.AppendLine("\tpublic static void LoadData()");
        code.AppendLine("\t{");
        foreach (var info in infos)
        {
            code.AppendLine($"\t\t{info.Name} = new TableAccessor{(info.Dic ? "Dictionary" : "List")}<Table.{info.Name}Enum, Table.{info.Name}Data>();");
        }
        code.AppendLine("\t}");
        code.AppendLine("}");

        File.WriteAllText(TableFolder + "TableAccessor.cs", code.ToString());
    }


    [MenuItem("ExcelBuilder/LoadTableData")]
    static void CreateTableData()
    {
        var builderData = Resources.Load<ExcelBuilderSO>($"Ref/ExcelBuilderData");
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

        foreach (var file in Directory.CreateDirectory(TableExcelFolder).GetFiles("*.xlsx", SearchOption.AllDirectories))
        {
            //LoadData
            var steam = File.OpenRead(file.FullName);
            var reader = ExcelReaderFactory.CreateOpenXmlReader(steam);
            var tables = reader.AsDataSet().Tables;
            reader.Close();
            steam.Close();

            foreach (DataTable table in tables)
            {
                if (table.Columns.Count < 2 || table.Rows.Count < 3)
                {
                    continue;
                }
                var tableName = GetTableName(table.TableName, tables[0].TableName, file.Name);

                //CreateTableData
                var assetName = tableName + "Data";
                var tableData = Resources.Load<ScriptableObjectBase>($"ExcelData/{assetName}");
                if (!tableData)
                {
                    tableData = (ScriptableObjectBase)ScriptableObject.CreateInstance(Type.GetType(tableName + "SO,Assembly-CSharp"));
                    AssetDatabase.CreateAsset(tableData, $"Assets/Resources/ExcelData/{assetName}.asset");
                }
                tableData.CreateData(table);
                EditorUtility.SetDirty(tableData);
                AssetDatabase.SaveAssets();
            }
        }
        Debug.Log("Load table data end.");
    }
    #endregion

    [InitializeOnLoadMethod]
    public static void OnLoadMethod()
    {
        var builderData = Resources.Load<ExcelBuilderSO>($"Ref/ExcelBuilderData");
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