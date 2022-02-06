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
    static string MessageFolder = Application.dataPath + "/Scripts/Message/";
    public static string TableFolder = Application.dataPath + "/Scripts/Table/";
    public static string RefFolder = Application.dataPath + "/Resources/Temp/";
    public static string DataFolder = Application.dataPath + "/Resources/ExcelData/";

    static string MsgExcelPath = Application.dataPath.Replace("Assets", "Data/Message/Message.xlsx");
    static string TableExcelFolder = Application.dataPath.Replace("Assets", "Data/Table/");

    private static void CreateFolder()
    {
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

        //CreateMsgData
        var msgData = Resources.Load<MessageSO>("ExcelData/MsgData");
        if (!msgData)
        {
            msgData = ScriptableObject.CreateInstance<MessageSO>();
            AssetDatabase.CreateAsset(msgData, "Assets/Resources/ExcelData/MsgData.asset");
        }
        msgData.CreateData(tables, msgLabels.Count);
        EditorUtility.SetDirty(msgData);
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
        Debug.Log("Msg build and load end.");
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
        var msgRef = Resources.Load<LabelRefSO>("Temp/MsgRef");
        if (!msgRef)
        {
            msgRef = ScriptableObject.CreateInstance<LabelRefSO>();
            AssetDatabase.CreateAsset(msgRef, "Assets/Resources/Temp/MsgRef.asset");
        }

        var code = new StringBuilder();
        code.AppendLine("public enum MsgLabel");
        code.AppendLine("{");
        foreach (var label in msgLabels)
        {
            code.AppendLine($"\t{label} = {msgRef.AddLabel(label)},");
        }
        code.AppendLine("}");

        EditorUtility.SetDirty(msgRef);
        AssetDatabase.SaveAssets();

        File.WriteAllText(MessageFolder + "MsgLabel.cs", code.ToString());
    }
    #endregion

    #region BuildTable
    class Field
    {
        public string[] Type;
        public string Name;
        public int ListCount = 0;
        public bool IsBaseType;

        public Field(string[] type, string name, bool isBaseType)
        {
            Type = type;
            Name = name;
            IsBaseType = isBaseType;
        }
    }

    static string GetTableName(string name, string table0Name, string fileName)
    {
        return (name == "Sheet1" && name == table0Name) ? fileName.Replace(".xlsx", string.Empty) : name;
    }

    [MenuItem("ExcelBuilder/BuildTableAndLoad")]
    private static void BuildTables()
    {
        CreateFolder();

        List<string> folderNames = new List<string>();
        List<DataTableCollection> excelData = new List<DataTableCollection>();

        var fileInfos = Directory.CreateDirectory(TableExcelFolder).GetFiles("*.xlsx");
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
                if (table.Columns.Count < 1 || table.Rows.Count < 3)
                {
                    continue;
                }
                var labels = new List<string>();
                for (int i = 2; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][0] is DBNull)
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
      
        //CreateDataClass
        List<string> classNames = new List<string>();

        for (int index = 0; index < fileInfos.Length; ++index)
        {
            foreach (DataTable table in excelData[index])
            {
                if (table.Columns.Count < 2 || table.Rows.Count < 2)
                {
                    continue;
                }
                List<Field> fields = new List<Field>();
                int listCount = 0;
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    var type = table.Rows[0][i].ToString();
                    if (type == "[")
                    {
                        if (fields.Count > 0 && listCount == 0)
                        {
                            listCount = 1;
                            continue;
                        }
                        else
                        {
                            Debug.LogError($"Invaild \"[\" at [{table.TableName}]");
                            return;
                        }
                    }
                    if (listCount > 0)
                    {
                        listCount++;
                        if (type == "]")
                        {
                            fields[fields.Count - 1].ListCount = listCount;
                            listCount = 0;
                        }
                        continue;
                    }
                    if (listCount == 0 && (type == string.Empty || table.Rows[1][i] is DBNull))
                    {
                        Debug.LogError($"Table has empty type or field! [{fileInfos[index].Name}]");
                    }
                    var typeList = type.Split(';');
                    var isBaseType = TypeConvert.SupportType.Contains(typeList[0]);
                    if (typeList.Length == 1)
                    {
                        if (!isBaseType && !folderNames.Contains(typeList[0]))
                        {
                            Debug.LogError($"There are not support type! [{typeList[0]}]");
                            return;
                        }
                    }
                    else
                    {
                        foreach (var t in typeList)
                        {
                            if (!folderNames.Contains(t))
                            {
                                Debug.LogError($"Muilt type must be table enum! [{type}-{t}]");
                                return;
                            }
                        }
                    }
                    fields.Add(new Field(typeList, table.Rows[1][i].ToString(), isBaseType));
                }
                if (listCount > 0)
                {
                    Debug.LogError($"Miss \"]\" at [{table.TableName}]");
                    return;
                }

                var tableName = GetTableName(table.TableName, excelData[index][0].TableName, fileInfos[index].Name);
                if (!folderNames.Contains(tableName))
                {
                    folderNames.Add(tableName);
                }

                CreateTableDataClass(tableName, fields);
                CreateTableSO(tableName, fields);

                classNames.Add(tableName);
            }
        }
        CreateTableAccessor(classNames);

        var builderData = Resources.Load<ExcelBuilderSO>("Temp/ExcelBuilderData");
        if (!builderData)
        {
            builderData = ScriptableObject.CreateInstance<ExcelBuilderSO>();
            AssetDatabase.CreateAsset(builderData, "Assets/Resources/Temp/ExcelBuilderData.asset");
        }
        builderData.Updata(folderNames);
        builderData.NeedRebuild = true;
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
            tabelRef = Resources.Load<LabelRefSO>($"Temp/{fileName}");
            if (!tabelRef)
            {
                tabelRef = ScriptableObject.CreateInstance<LabelRefSO>();
                AssetDatabase.CreateAsset(tabelRef, $"Assets/Resources/Temp/{fileName}.asset");
            }
        }

        var code = new StringBuilder();
        code.AppendLine("namespace Table");
        code.AppendLine("{");
        code.AppendLine($"\tpublic enum {name}");
        code.AppendLine("\t{");
        foreach (var field in labels)
        {
            if (needRef)
            {
                code.AppendLine($"\t\t{field} = {tabelRef.AddLabel(field)},");
            }
            else
            {
                code.AppendLine($"\t\t{field},");
            }
        }
        code.AppendLine("\t}");
        code.AppendLine("}");

        if (needRef)
        {
            EditorUtility.SetDirty(tabelRef);
            AssetDatabase.SaveAssets();
        }

        File.WriteAllText(folder + name + ".cs", code.ToString());
    }

    private static void CreateTableDataClass(string name, List<Field> fields)
    {
        Func<Field, string> GetType = (field) =>
        {
            var type = field.IsBaseType ? field.Type[0] : "int";
            return field.ListCount == 0 ? type : $"List<{type}>";
        };

        var folder = TableFolder + name + "/";
        Directory.CreateDirectory(folder);

        var code = new StringBuilder(); 
        code.AppendLine("using System.Collections.Generic;");
        code.AppendLine();
        code.AppendLine("namespace Table");
        code.AppendLine("{");
        code.AppendLine("\t[System.Serializable]");
        code.AppendLine($"\tpublic struct {name}Data");
        code.AppendLine("\t{");
        foreach (var field in fields)
        {
            code.AppendLine($"\t\tpublic {GetType(field)} {field.Name};");
        }
        code.AppendLine();
        code.Append($"\t\tpublic {name}Data({GetType(fields[0])} {fields[0].Name}");
        for (int i = 1; i < fields.Count; i++)
        {
            code.Append($", {GetType(fields[i])} {fields[i].Name}");
        }
        code.AppendLine(")");
        code.AppendLine("\t\t{");
        foreach (var field in fields)
        {
            code.AppendLine($"\t\t\tthis.{field.Name} = {field.Name};");
        }
        code.AppendLine("\t\t}");
        code.AppendLine("\t}");
        code.AppendLine("}");

        File.WriteAllText(folder + name + "Data.cs", code.ToString());
    }

    private static void CreateTableSO(string name, List<Field> fields)
    {
        Func<Field, string> GetTypes = (field) =>
        {
            var ret = new StringBuilder();
            ret.Append($"new string[] {{\"{field.Type[0]}\"");
            for (int i = 1; i < field.Type.Length; ++i)
            {
                ret.Append($", \"{field.Type[i]}\"");
            }
            ret.Append("}");
            return ret.ToString();
        };

        Func<Field, int, string> GetFieldValue = (field, index) =>
        {
            if (field.IsBaseType)
            {
                return field.ListCount == 0
                    ? $"\t\t\t\tTypeConvert.GetValue<{field.Type[0]}>(table.Rows[i][{index}].ToString())"
                    : $"\t\t\t\tnew List<{field.Type[0]}>()";
            }
            else
            {
                return field.ListCount == 0
                    ? $"\t\t\t\tTypeConvert.GetValue(table.Rows[i][{index}].ToString(), {GetTypes(field)})"
                    : $"\t\t\t\tnew List<int>()";
            }
        };
        var code = new StringBuilder();
        code.AppendLine("using System.Collections.Generic;");
        code.AppendLine("using System.Data;");
        code.AppendLine("using Table;");
        code.AppendLine();
        code.AppendLine($"public class {name}SO : ScriptableObjectBase");
        code.AppendLine("{");
        code.AppendLine($"\tpublic List<{name}Data> Datas;");
        code.AppendLine();
        code.AppendLine("\tpublic override void CreateData(DataTable table)");
        code.AppendLine("\t{");
        code.AppendLine($"\t\tDatas = new List<{name}Data>();");
        code.AppendLine("\t\tfor (int i = 2; i < table.Rows.Count; i++)");
        code.AppendLine("\t\t{");
        code.AppendLine("\t\t\tif (table.Rows[i][0] is System.DBNull)");
        code.AppendLine("\t\t\t{");
        code.AppendLine("\t\t\t\tcontinue;");
        code.AppendLine("\t\t\t}");
        code.AppendLine($"\t\t\tvar data = new {name}Data(");
        int index = 0;
        for (int i = 0; i < fields.Count - 1; ++i)
        {
            index += fields[i].ListCount + 1;
            code.AppendLine(GetFieldValue(fields[i], index) +", ");
        }
        index += fields[fields.Count - 1].ListCount + 1;
        code.AppendLine(GetFieldValue(fields[fields.Count - 1], index));
        code.AppendLine("\t\t\t\t);");
        index = 0;
        var firstList = true;
        foreach (var field in fields)
        {
            index++;
            if (field.ListCount > 0)
            {
                if (!field.IsBaseType)
                {
                    code.Append("\t\t\t");
                    if (firstList)
                    {
                        code.Append("var ");
                        firstList = false;
                    }
                    code.AppendLine($"types = {GetTypes(field)};");
                }
                code.AppendLine($"\t\t\tfor (int j = {index + 1}; j < {index + field.ListCount + 1}; j++)");
                code.AppendLine("\t\t\t{");
                code.AppendLine("\t\t\t\tif (table.Rows[i][j] is System.DBNull)");
                code.AppendLine("\t\t\t\t{");
                code.AppendLine("\t\t\t\t\tbreak;");
                code.AppendLine("\t\t\t\t}");
                code.AppendLine($"\t\t\t\tdata.{field.Name}.Add({(field.IsBaseType ? $"TypeConvert.GetValue<{field.Type[0]}>(table.Rows[i][j].ToString())" : "TypeConvert.GetValue(table.Rows[i][j].ToString(), types)")});");
                code.AppendLine("\t\t\t}");
                index += field.ListCount;
            }
        }
        code.AppendLine("\t\t\tDatas.Add(data);");
        code.AppendLine("\t\t}");
        code.AppendLine("\t}");
        code.AppendLine("}");
        File.WriteAllText(TableFolder + name + "/" + name + "SO.cs", code.ToString());
    }

    private static void CreateTableAccessor(List<string> names)
    {
        var code = new StringBuilder();
        code.AppendLine("using Table;");
        code.AppendLine();
        code.AppendLine("public static class TableAccessor");
        code.AppendLine("{");
        foreach (var name in names)
        {
            code.AppendLine($"\tpublic static TableAccessorBase<{name}Data, {name}> {name};");
        }
        code.AppendLine();
        code.AppendLine("\tpublic static void LoadData()");
        code.AppendLine("\t{");
        foreach (var name in names)
        {
            code.AppendLine($"\t\t{name} = new TableAccessorBase<{name}Data, {name}>();");
        }
        code.AppendLine("\t}");
        code.AppendLine("}");

        File.WriteAllText(TableFolder + "TableAccessor.cs", code.ToString());
    }


    [MenuItem("ExcelBuilder/LoadTableData")]
    static void CreateDataAssets()
    {
        var builderData = Resources.Load<ExcelBuilderSO>($"Temp/ExcelBuilderData");
        if (!builderData)
        {
            Debug.LogError("Please build first.");
            return;
        }
        if (builderData.NeedRebuild)
        {
            builderData.NeedRebuild = false;
            EditorUtility.SetDirty(builderData);
            AssetDatabase.SaveAssets();
        }

        foreach (var file in Directory.CreateDirectory(TableExcelFolder).GetFiles("*.xlsx"))
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


    [InitializeOnLoadMethod]
    public static void OnLoadMethod()
    {
        var builderData = Resources.Load<ExcelBuilderSO>($"Temp/ExcelBuilderData");
        if (!builderData || builderData.NeedRebuild == false)
        {
            return;
        }
        builderData.NeedRebuild = false;
        EditorUtility.SetDirty(builderData);
        AssetDatabase.SaveAssets();
        CreateDataAssets();
    }
    #endregion
}