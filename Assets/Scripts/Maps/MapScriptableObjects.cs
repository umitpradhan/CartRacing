using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapScriptableObject", menuName = "ScriptableObjects/MapScriptableObject")]
public class MapScriptableObjects : ScriptableObject
{
    public string mapName;
    public MapPathSO[] paths;
    public MapObstacleSO[] obstacles;
    public MapDecorationSO[] decorations;
}

[System.Serializable]
public struct MapPathSO
{
    public Vector2[] points; // series of path points the car must follow
}

[System.Serializable]
public struct MapObstacleSO
{
    public Vector2 position;
    public string obstacleType; // could map to a prefab
}

[System.Serializable]
public struct MapDecorationSO
{
    public Vector2 position;
    public string visualName; // cones, signs, etc.
}
//{
//    public List<MapData> MapDatas;
//public GameObject TileOverlay;
//public Color DefaultTileColor;
//public Color SpawnableTileColor;
//public Color NonSpawnableTileColor;
//}

//[System.Serializable]
//public struct MapData
//{
//    public int MapID;
//    public Grid MapPrefab;
//    public Vector3 SpawningPoint;
//    public List<Vector3> WayPoints;
//}
