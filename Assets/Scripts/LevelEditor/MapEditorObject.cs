//using UnityEngine;
//using System.Collections.Generic;

//public class MapEditorObject : MonoBehaviour
//{
//    //public string mapName = "NewMap";

//    //public List<Vector2> pathPoints = new List<Vector2>();
//    //public List<MapObstacle> obstacles = new List<MapObstacle>();
//    //public List<MapDecoration> decorations = new List<MapDecoration>();

//    //// Optional: Visual handles
//    //private void OnDrawGizmos()
//    //{
//    //    Gizmos.color = Color.yellow;
//    //    for (int i = 0; i < pathPoints.Count - 1; i++)
//    //    {
//    //        Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
//    //    }

//    //    Gizmos.color = Color.red;
//    //    foreach (var o in obstacles)
//    //        Gizmos.DrawSphere(o.position, 0.3f);

//    //    Gizmos.color = Color.cyan;
//    //    foreach (var d in decorations)
//    //        Gizmos.DrawCube(d.position, Vector3.one * 0.5f);
//    //}
//}

// LevelEditorTool.cs
// Attach this script to an empty GameObject in the scene
// Make sure to create and assign a MapEditorSettingScriptableObject ScriptableObject with references to your prefab categories

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LevelEditorTool : EditorWindow
{
    private MapEditorSettingScriptableObject editorSettings;
    private GameObject selectedPrefab;
    private Vector2 scrollPosition;
    private float gridSize = 1f;
    private string savePath = "Assets/LevelData.json";

    private List<GameObject> placedObjects = new List<GameObject>();

    //[MenuItem("Tools/Level Editor Tool")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorTool>("Level Editor");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        editorSettings = (MapEditorSettingScriptableObject)EditorGUILayout.ObjectField("Editor Settings", editorSettings, typeof(MapEditorSettingScriptableObject), false);

        if (editorSettings == null)
        {
            EditorGUILayout.HelpBox("Assign MapEditorSettingScriptableObject first.", MessageType.Warning);
            EditorGUILayout.EndScrollView();
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);

        foreach (var category in editorSettings.categories)
        {
            EditorGUILayout.LabelField(category.categoryName, EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            foreach (var prefab in category.prefabs)
            {
                if (prefab == null) continue;

                Texture2D preview = AssetPreview.GetAssetPreview(prefab);
                if (GUILayout.Button(preview != null ? preview : Texture2D.grayTexture, GUILayout.Width(64), GUILayout.Height(64)))
                {
                    selectedPrefab = prefab;
                    SceneView.RepaintAll();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Save Level to JSON"))
        {
            SaveLevelToJson();
        }

        if (GUILayout.Button("Clear Scene Preview"))
        {
            ClearPreview();
        }

        EditorGUILayout.EndScrollView();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (selectedPrefab == null || editorSettings == null)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = hit.point;
            pos = new Vector3(
                Mathf.Round(pos.x / gridSize) * gridSize,
                Mathf.Round(pos.y / gridSize) * gridSize,
                Mathf.Round(pos.z / gridSize) * gridSize
            );

            Handles.color = Color.green;
            Handles.DrawWireCube(pos, selectedPrefab.transform.localScale);

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                GameObject placed = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                Undo.RegisterCreatedObjectUndo(placed, "Place Prefab");
                placed.transform.position = pos;
                placed.transform.rotation = Quaternion.identity;
                placedObjects.Add(placed);
                e.Use();
            }

            if (e.type == EventType.MouseDown && e.button == 1 && !e.alt)
            {
                Collider[] colliders = Physics.OverlapSphere(pos, gridSize * 0.5f);
                foreach (var c in colliders)
                {
                    Undo.DestroyObjectImmediate(c.gameObject);
                }
                e.Use();
            }
        }
    }

    private void ClearPreview()
    {
        foreach (var obj in placedObjects)
        {
            if (obj != null)
                DestroyImmediate(obj);
        }
        placedObjects.Clear();
    }

    private void SaveLevelToJson()
    {
        List<LevelDataEntry> levelData = new List<LevelDataEntry>();

        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                levelData.Add(new LevelDataEntry
                {
                    prefabPath = path,
                    position = obj.transform.position,
                    rotation = obj.transform.eulerAngles
                });
            }
        }

        string json = JsonUtility.ToJson(new LevelDataWrapper { entries = levelData.ToArray() }, true);
        File.WriteAllText(savePath, json);
        AssetDatabase.Refresh();
    }
}
#endif
