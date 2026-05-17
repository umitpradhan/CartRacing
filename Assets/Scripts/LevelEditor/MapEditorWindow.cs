//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;
//using UnityEditor.SceneManagement;
//using System.Collections.Generic;
//using System.IO;

//public class LevelEditorWindow : EditorWindow
//{
//    private GameObject selectedPrefab;
//    private string levelName = "NewLevel";
//    private LevelObjectType currentLayer = LevelObjectType.Road;
//    private float rotation = 0f;
//    private Vector2Int gridSnap = new(1, 1);
//    private LevelData levelData = new();

//    [MenuItem("Tools/Level Editor")]
//    public static void Open()
//    {
//        GetWindow<LevelEditorWindow>("Level Editor");
//    }

//    private void OnEnable()
//    {
//        SceneView.duringSceneGui += OnSceneGUI;
//        levelData = new LevelData();
//    }

//    private void OnDisable()
//    {
//        SceneView.duringSceneGui -= OnSceneGUI;
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label(" Level Editor", EditorStyles.boldLabel);

//        selectedPrefab = EditorGUILayout.ObjectField("Prefab to Place", selectedPrefab, typeof(GameObject), false) as GameObject;
//        currentLayer = (LevelObjectType)EditorGUILayout.EnumPopup("Layer", currentLayer);
//        levelName = EditorGUILayout.TextField("Level Name", levelName);

//        GUILayout.Space(5);
//        EditorGUILayout.LabelField("Snap Settings");
//        gridSnap.x = EditorGUILayout.IntField("X Snap", gridSnap.x);
//        gridSnap.y = EditorGUILayout.IntField("Y Snap", gridSnap.y);

//        GUILayout.Space(10);
//        if (GUILayout.Button("Clear Placed Objects"))
//        {
//            if (EditorUtility.DisplayDialog("Confirm", "Clear all placed objects in scene?", "Yes", "No"))
//            {
//                foreach (var go in FindObjectsOfType<LevelEditorTag>())
//                    DestroyImmediate(go.gameObject);
//                levelData.objects.Clear();
//            }
//        }

//        if (GUILayout.Button("Export Level to JSON"))
//        {
//            string folder = Application.streamingAssetsPath + "/Levels";
//            Directory.CreateDirectory(folder);
//            string json = JsonUtility.ToJson(levelData, true);
//            File.WriteAllText(Path.Combine(folder, levelName + ".json"), json);
//            Debug.Log($"✅ Saved: {folder}/{levelName}.json");
//            EditorUtility.DisplayDialog("Saved", "Level exported successfully!", "OK");
//        }

//        GUILayout.Space(10);
//        GUILayout.Label("🧾 JSON Preview", EditorStyles.boldLabel);
//        EditorGUILayout.TextArea(JsonUtility.ToJson(levelData, true), GUILayout.Height(200));
//    }

//    private void OnSceneGUI(SceneView sceneView)
//    {
//        if (selectedPrefab == null)
//            return;

//        Event e = Event.current;
//        Vector3 worldMouse = GetMouseWorldPosition(e.mousePosition);
//        Vector3 snappedPos = new Vector3(
//            Mathf.Round(worldMouse.x / gridSnap.x) * gridSnap.x,
//            Mathf.Round(worldMouse.y / gridSnap.y) * gridSnap.y,
//            0
//        );

//        // Preview Ghost
//        Handles.color = new Color(0, 1, 0, 0.5f);
//        Handles.DrawWireCube(snappedPos, selectedPrefab.transform.localScale);

//        // Rotate
//        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
//        {
//            rotation += 90f;
//            if (rotation >= 360f) rotation = 0f;
//            e.Use();
//            SceneView.RepaintAll();
//        }

//        // Place on Left Click
//        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
//        {
//            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
//            obj.transform.position = snappedPos;
//            obj.transform.rotation = Quaternion.Euler(0, 0, rotation);
//            obj.name = selectedPrefab.name;
//            obj.AddComponent<PlacedObject>().objectType = currentLayer;

//            Undo.RegisterCreatedObjectUndo(obj, "Place Prefab");

//            levelData.objects.Add(new PlacedObjectData
//            {
//                prefabName = selectedPrefab.name,
//                position = snappedPos,
//                rotationZ = rotation,
//                layer = currentLayer
//            });

//            e.Use();
//        }

//        // Right Click Delete
//        if (e.type == EventType.MouseDown && e.button == 1 && !e.alt)
//        {
//            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//            if (Physics.Raycast(ray, out RaycastHit hit))
//            {
//                GameObject hitObj = hit.collider.gameObject;
//                if (hitObj.GetComponent<PlacedObject>())
//                {
//                    PlacedObject tag = hitObj.GetComponent<PlacedObject>();
//                    levelData.objects.RemoveAll(o =>
//                        o.type == hitObj.name &&
//                        Vector3.Distance(o.position, hitObj.transform.position) < 0.1f &&
//                        Mathf.Abs(o.rotationZ - hitObj.transform.eulerAngles.z) < 0.1f
//                    );
//                    Undo.DestroyObjectImmediate(hitObj);
//                    e.Use();
//                }
//            }
//        }
//    }

//    private Vector3 GetMouseWorldPosition(Vector2 mousePosition)
//    {
//        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
//        Plane plane = new Plane(Vector3.forward, Vector3.zero);
//        if (plane.Raycast(ray, out float dist))
//            return ray.GetPoint(dist);
//        return Vector3.zero;
//    }
//}
//#endif

//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using System.IO;

//public class MapEditorWindow : EditorWindow
//{
//    private MapEditorSettingScriptableObject editorSettings;
//    private GameObject selectedPrefab;
//    private Vector2 scrollPosition;
//    private float gridSize = 1f;
//    private float rotationSnap = 90f;
//    private string savePath = "Assets/LevelData.json";

//    private List<GameObject> placedObjects = new();

//    [MenuItem("Tools/Level Editor Tool")]
//    public static void ShowWindow()
//    {
//        GetWindow<LevelEditorTool>("Level Editor");
//    }

//    private void OnGUI()
//    {
//        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

//        // Editor Settings reference
//        EditorGUILayout.LabelField("Level Editor Settings", EditorStyles.boldLabel);
//        editorSettings = (MapEditorSettingScriptableObject)EditorGUILayout.ObjectField("Editor Settings", editorSettings, typeof(MapEditorSettingScriptableObject), false);

//        if (editorSettings == null)
//        {
//            EditorGUILayout.HelpBox("Assign the MapEditorSettingScriptableObject first.", MessageType.Warning);
//            EditorGUILayout.EndScrollView();
//            return;
//        }

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Prefab Categories", EditorStyles.boldLabel);

//        if (editorSettings.categories != null && editorSettings.categories.Count > 0)
//        {
//            foreach (var category in editorSettings.categories)
//            {
//                EditorGUILayout.LabelField(category.categoryName, EditorStyles.miniBoldLabel);
//                EditorGUILayout.BeginHorizontal();
//                foreach (var prefab in category.prefabs)
//                {
//                    Texture2D preview = AssetPreview.GetAssetPreview(prefab);
//                    if (GUILayout.Button(preview != null ? preview : Texture2D.grayTexture, GUILayout.Width(64), GUILayout.Height(64)))
//                    {
//                        selectedPrefab = prefab;
//                        SceneView.RepaintAll();
//                    }
//                }
//                EditorGUILayout.EndHorizontal();
//                EditorGUILayout.Space();
//            }
//        }
//        else
//        {
//            EditorGUILayout.HelpBox("No prefab categories found in the settings asset.", MessageType.Info);
//        }

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Editor Options", EditorStyles.boldLabel);

//        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
//        rotationSnap = EditorGUILayout.FloatField("Rotation Snap", rotationSnap);
//        savePath = EditorGUILayout.TextField("Save Path", savePath);

//        EditorGUILayout.Space();

//        EditorGUILayout.BeginHorizontal();
//        if (GUILayout.Button("Save Level to JSON"))
//        {
//            SaveLevelToJson();
//        }

//        if (GUILayout.Button("Clear Scene Preview"))
//        {
//            ClearPreview();
//        }
//        EditorGUILayout.EndHorizontal();

//        EditorGUILayout.EndScrollView();
//    }


//    private void PlacePrefab(GameObject prefab)
//    {
//        // Instantiate prefab at current Scene view position
//        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
//        Undo.RegisterCreatedObjectUndo(obj, "Placed Prefab");
//        Selection.activeGameObject = obj;
//    }

//    private void OnEnable()
//    {
//        SceneView.duringSceneGui += OnSceneGUI;
//    }

//    private void OnDisable()
//    {
//        SceneView.duringSceneGui -= OnSceneGUI;
//    }

//    private void OnSceneGUI(SceneView sceneView)
//    {
//        Event e = Event.current;
//        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

//        if (selectedPrefab == null || editorSettings == null)
//            return;

//        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//        if (Physics.Raycast(ray, out RaycastHit hit))
//        {
//            Vector3 pos = hit.point;
//            pos = new Vector3(
//                Mathf.Round(pos.x / gridSize) * gridSize,
//                Mathf.Round(pos.y / gridSize) * gridSize,
//                Mathf.Round(pos.z / gridSize) * gridSize
//            );

//            Handles.color = Color.green;
//            Handles.DrawWireCube(pos, selectedPrefab.transform.localScale);

//            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
//            {
//                GameObject placed = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
//                Undo.RegisterCreatedObjectUndo(placed, "Place Prefab");
//                placed.transform.position = pos;
//                placed.transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(placed.transform.eulerAngles.z / rotationSnap) * rotationSnap);
//                placedObjects.Add(placed);
//                e.Use();
//            }

//            if (e.type == EventType.MouseDown && e.button == 1 && !e.alt)
//            {
//                Collider[] colliders = Physics.OverlapSphere(pos, gridSize * 0.5f);
//                foreach (var c in colliders)
//                {
//                    Undo.DestroyObjectImmediate(c.gameObject);
//                }
//                e.Use();
//            }
//        }
//    }

//    private void ClearPreview()
//    {
//        foreach (var obj in placedObjects)
//        {
//            if (obj != null)
//                DestroyImmediate(obj);
//        }
//        placedObjects.Clear();
//    }

//    private void SaveLevelToJson()
//    {
//        List<LevelDataEntry> levelData = new();

//        foreach (var transform in FindObjectsOfType<Transform>())
//        {
//            GameObject obj = transform.gameObject;

//            if (PrefabUtility.IsPartOfPrefabInstance(obj))
//            {
//                GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(obj);
//                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root);
//                levelData.Add(new LevelDataEntry
//                {
//                    prefabPath = path,
//                    position = obj.transform.position,
//                    rotation = obj.transform.eulerAngles
//                });
//            }
//        }


//        string json = JsonUtility.ToJson(new LevelDataWrapper { entries = levelData.ToArray() }, true);
//        File.WriteAllText(savePath, json);
//        AssetDatabase.Refresh();
//    }

//}


//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor.SceneManagement;

//public class MapEditorWindow : EditorWindow
//{
//    private Vector2 scrollPosition;
//    private GameObject selectedPrefab;

//    private float gridSize = 1f;
//    private float rotationSnap = 90f;
//    private float currentRotation = 0f;
//    private float rotation = 0f;

//    private string savePath = "Assets/level.json";

//    private MapEditorSettingScriptableObject editorSettings;

//    [MenuItem("Tools/Map Editor")]
//    public static void ShowWindow()
//    {
//        GetWindow<MapEditorWindow>("Map Editor");
//    }

//    private void OnEnable()
//    {
//        SceneView.duringSceneGui += OnSceneGUI;
//    }

//    private void OnDisable()
//    {
//        SceneView.duringSceneGui -= OnSceneGUI;
//    }

//    private void OnGUI()
//    {
//        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

//        // Load ScriptableObject Settings
//        editorSettings = (MapEditorSettingScriptableObject)EditorGUILayout.ObjectField("Editor Settings", editorSettings, typeof(MapEditorSettingScriptableObject), false);

//        if (editorSettings == null)
//        {
//            EditorGUILayout.HelpBox("Assign LevelEditorSettings first.", MessageType.Warning);
//            EditorGUILayout.EndScrollView();
//            return;
//        }

//        // Show Prefab Categories
//        foreach (var category in editorSettings.categories)
//        {
//            EditorGUILayout.LabelField(category.categoryName, EditorStyles.boldLabel);
//            EditorGUILayout.BeginHorizontal();
//            foreach (var prefab in category.prefabs)
//            {
//                if (prefab == null) continue;

//                Texture2D preview = AssetPreview.GetAssetPreview(prefab);
//                if (GUILayout.Button(preview != null ? preview : Texture2D.grayTexture, GUILayout.Width(64), GUILayout.Height(64)))
//                {
//                    selectedPrefab = prefab;
//                    SceneView.RepaintAll();
//                }
//            }
//            EditorGUILayout.EndHorizontal();
//        }

//        EditorGUILayout.Space();
//        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
//        rotationSnap = EditorGUILayout.FloatField("Rotation Snap", rotationSnap);
//        savePath = EditorGUILayout.TextField("Save Path", savePath);

//        if (GUILayout.Button("Save Level to JSON"))
//        {
//            SaveLevelToJson();
//        }

//        if (GUILayout.Button("Clear Scene Preview"))
//        {
//            ClearPreview();
//        }

//        EditorGUILayout.EndScrollView();
//    }

//    private void OnSceneGUI(SceneView sceneView)
//    {
//        Event e = Event.current;
//        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

//        if (e.type == EventType.MouseDown && e.button == 0 && selectedPrefab != null)
//        {
//            Vector3 worldPos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
//            Vector3 snappedPos = new Vector3(
//                Mathf.Round(worldPos.x / gridSize) * gridSize,
//                Mathf.Round(worldPos.y / gridSize) * gridSize,
//                0f
//            );

//            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
//            Undo.RegisterCreatedObjectUndo(obj, "Place Prefab");
//            obj.transform.position = snappedPos;
//            obj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(obj.transform.eulerAngles.z / rotationSnap) * rotationSnap);

//            e.Use();
//        }

//        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
//        {
//            rotation += 90f;
//            if (rotation >= 360f) rotation = 0f;
//            e.Use();
//            SceneView.RepaintAll();
//        }

//        // Right click to delete
//        if (e.type == EventType.MouseDown && e.button == 1)
//        {
//            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//            if (Physics.Raycast(ray, out RaycastHit hit))
//            {
//                Undo.DestroyObjectImmediate(hit.collider.gameObject);
//                e.Use();
//            }
//            else
//            {
//                // 2D fallback
//                Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
//                Collider2D hit2D = Physics2D.OverlapPoint(mousePos);
//                if (hit2D != null)
//                {
//                    Undo.DestroyObjectImmediate(hit2D.gameObject);
//                    e.Use();
//                }
//            }
//        }
//    }

//    //private void OnSceneGUI(SceneView sceneView)
//    //{
//    //    Event e = Event.current;

//    //    // Rotate selected prefab using 'R'
//    //    if (selectedPrefab != null && e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
//    //    {
//    //        currentRotation += rotationSnap;
//    //        e.Use(); // consume event
//    //    }

//    //    // Left-click to place prefab
//    //    if (selectedPrefab != null && e.type == EventType.MouseDown && e.button == 0)
//    //    {
//    //        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//    //        if (Physics.Raycast(ray, out RaycastHit hit))
//    //        {
//    //            Vector3 position = hit.point;
//    //            Vector3 snappedPos = new Vector3(
//    //                Mathf.Round(position.x / gridSize) * gridSize,
//    //                Mathf.Round(position.y / gridSize) * gridSize,
//    //                Mathf.Round(position.z / gridSize) * gridSize
//    //            );

//    //            GameObject placed = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
//    //            Undo.RegisterCreatedObjectUndo(placed, "Place Prefab");
//    //            placed.transform.position = snappedPos;
//    //            placed.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
//    //            Selection.activeGameObject = placed;

//    //            e.Use();
//    //        }
//    //    }

//    //    if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
//    //    {
//    //        rotation += 90f;
//    //        if (rotation >= 360f) rotation = 0f;
//    //        e.Use();
//    //        SceneView.RepaintAll();
//    //    }

//    //    // Right-click to delete object with tag "PlacedPrefab"
//    //    if (e.type == EventType.MouseDown && e.button == 1)
//    //    {
//    //        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//    //        if (Physics.Raycast(ray, out RaycastHit hit))
//    //        {
//    //            GameObject hitObj = hit.collider.gameObject;
//    //            if (PrefabUtility.IsPartOfAnyPrefab(hitObj))
//    //            {
//    //                Undo.DestroyObjectImmediate(hitObj);
//    //                e.Use();
//    //            }
//    //        }
//    //    }
//    //}

//    private void ClearPreview()
//    {
//        foreach (var obj in Object.FindObjectsOfType<GameObject>())
//        {
//            if (PrefabUtility.IsPartOfPrefabInstance(obj))
//            {
//                Undo.DestroyObjectImmediate(obj);
//            }
//        }
//    }

//    private void SaveLevelToJson()
//    {
//        List<LevelDataEntry> levelData = new();
//        foreach (var obj in Object.FindObjectsOfType<GameObject>())
//        {
//            if (PrefabUtility.IsPartOfPrefabInstance(obj))
//            {
//                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
//                levelData.Add(new LevelDataEntry
//                {
//                    prefabPath = path,
//                    position = obj.transform.position,
//                    rotation = obj.transform.eulerAngles
//                });
//            }
//        }

//        string json = JsonUtility.ToJson(new LevelDataWrapper { entries = levelData.ToArray() }, true);
//        File.WriteAllText(savePath, json);
//        AssetDatabase.Refresh();
//    }

//    //[System.Serializable]
//    //private class LevelDataEntry
//    //{
//    //    public string prefabPath;
//    //    public Vector3 position;
//    //    public Vector3 rotation;
//    //}

//    //[System.Serializable]
//    //private class LevelDataWrapper
//    //{
//    //    public LevelDataEntry[] entries;
//    //}
//}

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LevelEditorWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private GameObject selectedPrefab;
    private float gridSize = 1f;
    private float rotationSnap = 90f;
    private float rotation = 0f;
    private GameObject lastPlacedPrefab = null;
    private Vector3 lastPlacedPosition;
    private Bounds lastBounds;
    private string savePath = "Assets/level.json";

    private MapEditorSettingScriptableObject editorSettings;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        rotation = 0f;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Load ScriptableObject Settings
        editorSettings = (MapEditorSettingScriptableObject)EditorGUILayout.ObjectField("Editor Settings", editorSettings, typeof(MapEditorSettingScriptableObject), false);

        if (editorSettings == null)
        {
            EditorGUILayout.HelpBox("Assign LevelEditorSettings first.", MessageType.Warning);
            EditorGUILayout.EndScrollView();
            return;
        }

        // Show Prefab Categories
        foreach (var category in editorSettings.categories)
        {
            EditorGUILayout.LabelField(category.categoryName, EditorStyles.boldLabel);
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
        rotationSnap = EditorGUILayout.FloatField("Rotation Snap", rotationSnap);
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

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        //GameObject obj = null; 
        if (e.type == EventType.MouseDown && e.button == 0 && selectedPrefab != null)
        {
            Vector3 worldPos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
            //Vector3 snappedPos = new Vector3(
            //    Mathf.Round(worldPos.x / gridSize) * gridSize,
            //    Mathf.Round(worldPos.y / gridSize) * gridSize,
            //    -0.1f
            //);
            Vector3 snappedPos;

            if (lastPlacedPrefab != null)
            {
                // Get current prefab bounds
                Bounds newBounds = GetRendererBounds(selectedPrefab);
                float widthOffset = lastBounds.extents.y + newBounds.extents.y;

                // Snap to the end of last placed prefab (horizontally right)      *************************************
                snappedPos = new Vector3(
                    lastPlacedPosition.x ,
                    lastPlacedPosition.y + widthOffset,
                    -0.1f
                );
            }
            else
            {
                // First object – snap to grid normally
                snappedPos = new Vector3(
                    Mathf.Round(worldPos.x / gridSize) * gridSize,
                    Mathf.Round(worldPos.y / gridSize) * gridSize,
                    -0.1f
                );
            }

            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            Undo.RegisterCreatedObjectUndo(obj, "Place Prefab");
            obj.transform.position = snappedPos;
            obj.transform.rotation = Quaternion.Euler(0, 0, rotation);

            lastPlacedPrefab = obj;
            lastPlacedPosition = obj.transform.position;
            lastBounds = GetRendererBounds(obj); 
            AssignSortingLayer(obj, selectedPrefab);

            e.Use();
        }

        // Right click to delete
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Undo.DestroyObjectImmediate(hit.collider.gameObject);
                e.Use();
            }
            else
            {
                // 2D fallback
                Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
                Collider2D hit2D = Physics2D.OverlapPoint(mousePos);
                if (hit2D != null)
                {
                    Undo.DestroyObjectImmediate(hit2D.gameObject);
                    e.Use();
                }
            }
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
        {
            rotation = (rotation + rotationSnap) % 360f;
            if (lastPlacedPrefab != null)
            {
                Undo.RecordObject(lastPlacedPrefab.transform, "Rotate Placed Prefab");
                lastPlacedPrefab.transform.rotation = Quaternion.Euler(0, 0, rotation);
            }
            e.Use();
            SceneView.RepaintAll();
        }
    }

    private Bounds GetRendererBounds(GameObject prefab)
    {
        var renderer = prefab.GetComponent<SpriteRenderer>();
        if (renderer != null)
            return renderer.bounds;

        // If no SpriteRenderer found, return default
        return new Bounds(Vector3.zero, Vector3.one);
    }


    private void ClearPreview()
    {
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                Undo.DestroyObjectImmediate(obj);
            }
        }
    }

    private void SaveLevelToJson()
    {
        List<LevelDataEntry> levelData = new();
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
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

    private void AssignSortingLayer(GameObject placedObj, GameObject prefab)
    {
        var spriteRenderer = placedObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        string layerName = "Default";
        int orderInLayer = 0;

        // Try to find the category this prefab belongs to
        if (editorSettings != null)
        {
            foreach (var category in editorSettings.categories)
            {
                if (category.prefabs.Contains(prefab))
                {
                    // Use category name as sorting layer (you must define them in Unity's Tags & Layers settings)
                    layerName = category.categoryName;
                    orderInLayer = GetOrderFromCategory(category.categoryName);
                    break;
                }
            }
        }

        // Fallback: use prefab's tag if category not found
        if (layerName == "Default" && !string.IsNullOrEmpty(prefab.tag))
        {
            layerName = prefab.tag;
            orderInLayer = GetOrderFromCategory(prefab.tag);
        }

        spriteRenderer.sortingLayerName = layerName;
        spriteRenderer.sortingOrder = orderInLayer;
    }

    // Custom sorting order per category/tag (can be extended)
    private int GetOrderFromCategory(string category)
    {
        switch (category)
        {
            case "Ground": return 0;
            case "Obstacles": return 3;
            case "Decorations": return 2;
            case "StraightRoads":
            case "BendRoads": return 1;
            case "SpawnPoints": return 4;
            case "Enemies": return 6;
            case "Player": return 10;
            default: return 0;
        }
    }


    //[System.Serializable]
    //private class LevelDataEntry
    //{
    //    public string prefabPath;
    //    public Vector3 position;
    //    public Vector3 rotation;
    //}

    //[System.Serializable]
    //private class LevelDataWrapper
    //{
    //    public LevelDataEntry[] entries;
    //}
}
#endif
