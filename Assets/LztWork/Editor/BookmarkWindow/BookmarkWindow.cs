using UnityEngine;
using UnityEditor;

public class BookmarkWindow : EditorWindow
{
    private BookmarkData _bookmarkData;

    [MenuItem("Window/Bookmark Manager")]
    public static void ShowWindow()
    {
        GetWindow<BookmarkWindow>("Bookmark Manager");
    }

    private void OnEnable()
    {
        _bookmarkData = BookmarkData.LoadOrCreate();
    }

    private void OnGUI()
    {
        HandleDragAndDrop(Event.current);

        GUILayout.Label("Bookmarks:", EditorStyles.boldLabel);

        for (int i = 0; i < _bookmarkData.Bookmarks.Count; i++)
        {
            Object obj = _bookmarkData.Bookmarks[i];
            if (!obj)
            {
                RemoveBookmark(i);
                break;
            }

            EditorGUILayout.BeginHorizontal();
            _bookmarkData.Bookmarks[i] = EditorGUILayout.ObjectField(obj, typeof(Object), false);
            if (GUILayout.Button("Open", GUILayout.Width(50)))
            {
                AssetDatabase.OpenAsset(obj);
            }
            if (GUILayout.Button("✕", GUILayout.Width(20)))
            {
                RemoveBookmark(i);
                EditorGUILayout.EndHorizontal();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Clear All"))
        {
            _bookmarkData.Bookmarks.Clear();
            SaveData();
        }
    }

    private void HandleDragAndDrop(Event evt)
    {
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        if (!_bookmarkData.Bookmarks.Contains(obj))
                        {
                            _bookmarkData.Bookmarks.Add(obj);
                        }
                    }
                    SaveData();
                }
                Event.current.Use();
                break;
        }
    }

    private void RemoveBookmark(int index)
    {
        _bookmarkData.Bookmarks.RemoveAt(index);
        SaveData();
    }

    private void SaveData()
    {
        EditorUtility.SetDirty(_bookmarkData);
        AssetDatabase.SaveAssets();
    }
}
