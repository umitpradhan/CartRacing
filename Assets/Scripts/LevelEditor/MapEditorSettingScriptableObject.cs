// LevelEditorSettings.cs (ScriptableObject to hold category-wise prefab references)

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapEditorSettingScriptableObject", menuName = "ScriptableObjects/MapEditorSettingScriptableObject")]
public class MapEditorSettingScriptableObject : ScriptableObject
{
    public List<PrefabCategory> categories = new List<PrefabCategory>();
    //public List<GameObject> roadPrefabs;
    //public List<GameObject> groundPrefabs;
    //public List<GameObject> obstaclePrefabs;
    //public List<GameObject> decorationPrefabs;
    //public List<GameObject> spawnpointPrefabs;
}

[System.Serializable]
public class PrefabCategory
{
    public string categoryName;
    public List<GameObject> prefabs = new List<GameObject>();
}
