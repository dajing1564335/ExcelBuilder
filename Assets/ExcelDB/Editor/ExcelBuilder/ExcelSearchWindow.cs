using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Table
{
    public class ExcelSearchWindow : EditorWindow
    {
        public string SearchPattern;

        private Vector2 scrollPos;
        private readonly List<DataTable> _data = new();
        private readonly List<string> _result = new();

        [MenuItem("ExcelBuilder/SearchWindow")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ExcelSearchWindow)).Show();
        }

        private void OnEnable()
        {
            LoadData();
        }

        private void OnGUI()
        {
            SearchPattern = EditorGUILayout.TextField("SearchPattern", SearchPattern);

            if (GUILayout.Button("Search"))
            {
                Search();
            }
            if (GUILayout.Button("Reload"))
            {
                LoadData();
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var line in _result)
            {
                EditorGUILayout.LabelField(line);
            }
            EditorGUILayout.EndScrollView();
        }

        private void LoadData()
        {
            _data.Clear();
            foreach (var file in Directory.CreateDirectory(ExcelBuilder.TableExcelFolder).GetFiles("*.xlsx", SearchOption.AllDirectories).Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)))
            {
                using var steam = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = ExcelReaderFactory.CreateOpenXmlReader(steam);
                var tables = ExcelBuilder.RemoveComment(reader.AsDataSet().Tables);
                foreach (DataTable table in tables)
                {
                    if (table.Columns.Count < 2 || table.Rows.Count < 3 || table.TableName == ExcelBuilder.SkipSheetName)
                    {
                        continue;
                    }
                    table.TableName = ExcelBuilder.GetTableName(table.TableName, file.Name);
                    _data.Add(table);
                }
            }
        }

        private void Search()
        {
            _result.Clear();
            foreach (var table in _data)
            {
                for (int i = 2; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][0] is DBNull)
                    {
                        continue;
                    }
                    for (int j = 1; j < table.Columns.Count; j++)
                    {
                        if (table.Rows[i][j].ToString().Contains(SearchPattern))
                        {
                            _result.Add(table.TableName.PadRight(20) + table.Rows[i][0].ToString());
                        }
                    }
                }
            }
        }
    }
}